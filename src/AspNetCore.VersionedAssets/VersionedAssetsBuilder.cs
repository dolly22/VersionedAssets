using AspNetCore.VersionedAssets;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Configuration builder helper class
    /// </summary>
    public class VersionedAssetsBuilder
    {
        public VersionedAssetsBuilder(VersionedAssetsOptions options)
        {
            this.Options = options;
        }

        public VersionedAssetsOptions Options { get; private set; }
    }

    public static class VersionedAssetsBuilderExtensions
    {
        public static VersionedAssetsBuilder WithUrlPrefix(this VersionedAssetsBuilder builder, string urlPrefix)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Options.UrlPrefix = urlPrefix;
            return builder;
        }

        public static VersionedAssetsBuilder WithAssemblyHashGlobalVersion(this VersionedAssetsBuilder builder, Assembly assembly)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            using (var sha = SHA256.Create())
            {
                using (var readStream = File.OpenRead(assembly.Location))
                {
                    var hash = sha.ComputeHash(readStream);
                    builder.Options.GlobalVersion = WebEncoders.Base64UrlEncode(hash);
                }
            }
            return builder;
        }
    }
}
