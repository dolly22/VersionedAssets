using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.VersionedAssets
{
    public class VersionedAssetsClientCaching
    { 
        static readonly DateTime expiresNoCache = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        readonly VersionedAssetsCaching cachingOptions;

        public VersionedAssetsClientCaching(VersionedAssetsCaching cachingOptions)
        {
            if (cachingOptions == null)
                throw new ArgumentNullException(nameof(cachingOptions));
               
            this.cachingOptions = cachingOptions;
        }

        public void ApplyResponseHeaders(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var assetInfo = context.Features.Get<IAssetInfoFeature>();
            if (assetInfo == null)
                return;

            var headers = context.Response.GetTypedHeaders();
            if (assetInfo.UrlHashMatched)
            {
                // apply caching headers
                headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                {
                    Public = cachingOptions.Public,
                    MaxAge = cachingOptions.MaxAge
                };
                headers.Append("Vary", "Accept-Encoding");
            }
            else
            {
                // set no cache headers
                headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                {
                    MustRevalidate = true,
                    NoCache = true,
                    NoStore = true
                };
                headers.Expires = expiresNoCache;
                headers.Append("Pragma", "no-cache");
            }
        }
    }
}
