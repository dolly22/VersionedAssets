#I "packages/FAKE.Core/tools/"
#r "FakeLib.dll"
#I "packages/FAKE.Dotnet/tools/"
#r "Fake.Dotnet.dll"

#load "paket-files\dolly22\FAKE.Gitsemver\Gitsemver.fsx"

open Fake
open Fake.Git
open Fake.SemVerHelper
open Fake.Dotnet
open Gitsemver

let mutable version : SemVerHelper.SemVerInfo option = None
let mutable currentGitSha : string = ""

Target "Clean" (fun _ ->
    !! "artifacts" ++ "src/*/bin" ++ "test/*/bin"
        |> DeleteDirs
)

Target "DetermineVersion" (fun _ ->   
    let semver = 
        getSemverInfoDefault 
        |> appendPreReleaseBuildNumber 3

    version <- Some semver        
    currentGitSha <- getCurrentSHA1 currentDirectory

    let fileVersion = sprintf "%d.%d.%d" semver.Major semver.Minor semver.Patch
    let assemblyVersion = sprintf "%d.0.0" semver.Major

    tracefn "Using version: %A" version.Value
)

Target "PrepareDotnetCli" (fun _ ->
    let sdkVersion = GlobalJsonSdk "global.json"
    tracefn "Using global.json sdk version: %s" sdkVersion

    // set custom install directory
    let customInstallDirectory = environVar "LocalAppData" @@ "Microsoft" @@ "dotnetbld" @@ sdkVersion

    let setOptions (options: DotNetCliInstallOptions) = 
        { options with 
            Version = Version sdkVersion
            Channel = Beta
            CustomInstallDir = Some customInstallDirectory
        }    

    DefaultDotnetCliDir <- customInstallDirectory
    DotnetCliInstall setOptions
)

Target "RestorePackage" (fun _ ->
    DotnetRestore id (currentDirectory @@ "src")
)

Target "BuildProjects" (fun _ ->
    !! "src/*/project.json" 
        |> Seq.iter(fun proj -> 
            let versionSuffix = 
                match version with
                | Some x -> Some x.PreRelease.Value.Name
                | None -> None

            // build project and produce outputs
            DotnetPack (fun c -> 
                { c with 
                    Configuration = Release;
                    VersionSuffix = versionSuffix;
                    OutputPath = Some (currentDirectory @@ "artifacts")
                }) proj
        )
)

Target "Default" <| DoNothing

"Clean"
    ==> "DetermineVersion"
    ==> "PrepareDotnetCli"
    ==> "RestorePackage"
    ==> "BuildProjects"    
    ==> "Default"

// start build
RunTargetOrDefault "Default"