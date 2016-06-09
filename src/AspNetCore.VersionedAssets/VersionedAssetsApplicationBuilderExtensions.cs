using AspNetCore.VersionedAssets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class VersionedAssetsApplicationBuilderExtensions
    {
        public static VersionedAssetsBuilder UseVersionedAssets(this IApplicationBuilder appBuilder, string pathMatch = null, string globalVersion = null)
        {
            var options = appBuilder.ApplicationServices.GetRequiredService<IOptions<VersionedAssetsOptions>>();
            var configBuilder = new VersionedAssetsBuilder(options.Value);

            // exit early when disabled
            if (!options.Value.IsEnabled)
                return configBuilder;

            pathMatch = pathMatch ?? options.Value.PathMatch ?? "/static";

            // override global version from parameters
            if (!string.IsNullOrWhiteSpace(globalVersion))
                options.Value.GlobalVersion = globalVersion;

            appBuilder.Map(pathMatch, builder => {
                // initialize caching utils
                var caching = new VersionedAssetsClientCaching(options.Value.Caching);

                // strips hash from url and sets context feature with match information
                builder.UseMiddleware<VersionedAssetsMiddleware>();

                // use static files handler to serve assets
                builder.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = (fileResponse) => {
                        if (fileResponse.File.Exists)
                            caching.ApplyResponseHeaders(fileResponse.Context);
                    }
                });
            });

            return configBuilder;
        }
    }
}
