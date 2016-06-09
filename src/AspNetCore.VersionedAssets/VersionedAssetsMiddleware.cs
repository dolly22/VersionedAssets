using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.VersionedAssets
{
    public class VersionedAssetsMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;
        private readonly FileHashProvider hashProvider;
        private readonly VersionedAssetsOptions options;

        public VersionedAssetsMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            IHostingEnvironment hostingEnv,
            IMemoryCache cache,
            IOptions<VersionedAssetsOptions> options)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));
            if (hostingEnv == null)
                throw new ArgumentNullException(nameof(hostingEnv));
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.next = next;
            this.logger = loggerFactory.CreateLogger<VersionedAssetsMiddleware>();
            this.hashProvider = new FileHashProvider(hostingEnv.WebRootFileProvider, cache, new PathString());
            this.options = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            string assetHash;
            string assetPath;
            var hashMatched = false;

            if (TryExtractAssetHash(context.Request.Path, out assetHash, out assetPath))
            {
                // try to compare global hash
                if (string.Equals(options.GlobalVersion, assetHash, StringComparison.Ordinal))
                {
                    hashMatched = true;
                }
                else
                {
                    var computedHash = hashProvider.GetContentHash(assetPath);
                    hashMatched = string.Equals(assetHash, computedHash, StringComparison.Ordinal);
                }

                // rewrite request path
                context.Request.Path = new PathString(assetPath);  
                
                if (logger.IsEnabled(LogLevel.Trace))                         
                    logger.LogTrace($"VersionedAsset requested {assetPath}, url-hash:{assetHash}, matched:{hashMatched}");
            }
            else
            {
                logger.LogInformation("Unable to extract hash information from url");
            }

            // set request info
            context.Features.Set(hashMatched ? AssetInfoFeature.Matched : AssetInfoFeature.NotMatched);

            await next.Invoke(context);
        }


        private bool TryExtractAssetHash(string requestPath, out string hash, out string assetPath)
        {
            hash = null;
            assetPath = requestPath;

            var splitIndex = requestPath.IndexOf("/", 1);
            if (splitIndex > 0)
            {
                var head = requestPath.Substring(0, splitIndex);
                var tail = requestPath.Substring(splitIndex);

                hash = head.Substring(1);
                assetPath = tail;
                return true;
            }
            return false;       
        }
    }
}
