using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AspNetCore.VersionedAssets
{
    public class FileHashProvider
    {
        private static readonly char[] QueryStringAndFragmentTokens = new[] { '?', '#' };

        readonly IFileProvider fileProvider;
        readonly IMemoryCache cache;
        readonly PathString requestPathBase;

        public FileHashProvider(
            IFileProvider fileProvider,
            IMemoryCache cache,
            PathString requestPathBase)
        {
            if (fileProvider == null)
                throw new ArgumentNullException(nameof(fileProvider));
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            this.fileProvider = fileProvider;
            this.cache = cache;
            this.requestPathBase = requestPathBase;
        }

        public string GetContentHash(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var resolvedPath = path;

            var queryStringOrFragmentStartIndex = path.IndexOfAny(QueryStringAndFragmentTokens);
            if (queryStringOrFragmentStartIndex != -1)
                resolvedPath = path.Substring(0, queryStringOrFragmentStartIndex);

            Uri uri;
            if (Uri.TryCreate(resolvedPath, UriKind.Absolute, out uri) && !uri.IsFile)
            {
                // Don't append version if the path is absolute.
                return null;
            }

            string value;
            if (!cache.TryGetValue(path, out value))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions();
                cacheEntryOptions.AddExpirationToken(fileProvider.Watch(resolvedPath));
                var fileInfo = fileProvider.GetFileInfo(resolvedPath);

                if (!fileInfo.Exists
                    && requestPathBase.HasValue
                    && resolvedPath.StartsWith(requestPathBase.Value, StringComparison.OrdinalIgnoreCase))
                {
                    var requestPathBaseRelativePath = resolvedPath.Substring(requestPathBase.Value.Length);
                    cacheEntryOptions.AddExpirationToken(fileProvider.Watch(requestPathBaseRelativePath));
                    fileInfo = fileProvider.GetFileInfo(requestPathBaseRelativePath);
                }

                if (fileInfo.Exists)
                {
                    value = GetHashForFile(fileInfo);
                }
                else
                {
                    // if the file is not in the current server.
                    value = null;
                }

                value = cache.Set<string>(path, value, cacheEntryOptions);
            }

            return value;
        }

        private static string GetHashForFile(IFileInfo fileInfo)
        {
            using (var sha = SHA256.Create())
            {
                using (var readStream = fileInfo.CreateReadStream())
                {
                    var hash = sha.ComputeHash(readStream);
                    return WebEncoders.Base64UrlEncode(hash);
                }
            }
        }
    }
}
