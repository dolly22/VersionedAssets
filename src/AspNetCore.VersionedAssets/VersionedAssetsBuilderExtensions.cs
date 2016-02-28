using AspNetCore.VersionedAssets;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Builder
{
    public static class VersionedAssetsBuilderExtensions
    {
        public static void UseVersionedAssets(this IApplicationBuilder appBuilder, string pathMatch = "/static")
        {
            appBuilder.Map(pathMatch, builder => {
                var caching = new VersionedAssetsClientCaching();

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
        }
    }
}
