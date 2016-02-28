using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.VersionedAssets
{
    public class VersionedAssetsClientCaching
    {
        static readonly DateTime expiresNoCache = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void ApplyResponseHeaders(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var assetInfo = context.Features.Get<IAssetInfoFeature>();
            if (assetInfo == null)
                return;

            var headers = context.Response.GetTypedHeaders();
            if (assetInfo.HashMatched)
            {
                // apply caching headers
                headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(365)
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
