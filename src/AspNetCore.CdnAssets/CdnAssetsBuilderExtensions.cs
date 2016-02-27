using AspNetCore.CdnAssets;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Builder
{
    public static class CdnAssetsBuilderExtensions
    {
        public static void UseCdnAssets(this IApplicationBuilder appBuilder, string pathMatch)
        {
            appBuilder.Map(pathMatch, builder => {
                var caching = new CdnAssetsClientCaching();

                // strips hash from url and sets context feature with match information
                builder.UseMiddleware<CdnAssetsMiddleware>();

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
