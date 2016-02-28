﻿using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.CdnAssets
{
    public class CdnAssetsMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;
        private readonly FileHashProvider hashProvider;

        public CdnAssetsMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            IHostingEnvironment hostingEnv,
            IMemoryCache cache)
        {
            this.next = next;
            this.logger = loggerFactory.CreateLogger<CdnAssetsMiddleware>();
            this.hashProvider = new FileHashProvider(hostingEnv.WebRootFileProvider, cache, new PathString());
        }

        public async Task Invoke(HttpContext context)
        {
            string assetHash;
            string assetPath;

            if (TryExtractAssetHash(context.Request.Path, out assetHash, out assetPath))
            {
                var computedHash = hashProvider.GetContentHash(assetPath);
                var hashMatched = string.Equals(assetHash, computedHash, StringComparison.Ordinal);

                // rewrite request path
                context.Request.Path = new PathString(assetPath);

                context.Features.Set<IAssetInfoFeature>(new AssetInfoFeature
                {
                    HashMatched = hashMatched
                });
                logger.LogInformation($"Origin pull {assetPath}, url-hash:{assetHash}, file-hash:{computedHash}, matched:{hashMatched}");
            }
            else
            {
                logger.LogInformation("Unable to extract hash information from url");
            }
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