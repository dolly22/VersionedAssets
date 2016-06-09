using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.VersionedAssets
{
    public interface IVersionedAssetsOptions
    {
        bool IsEnabled { get; }

        /// <summary>
        /// Prefix content path
        /// </summary>
        string UrlPrefix { get; }

        /// <summary>
        /// Global assets version
        /// </summary>
        string GlobalVersion { get; }

        /// <summary>
        /// Always use gloval version as prefix (event prefix when file content version is requested)
        /// </summary>
        bool AlwaysPrefixGlobalVersion { get; }
    }

    public class VersionedAssetsOptions : IVersionedAssetsOptions
    {
        public bool IsEnabled { get; set; } = true;

        public string UrlPrefix { get; set; } = "/static";

        public string GlobalVersion { get; set; }

        public bool AlwaysPrefixGlobalVersion { get; set; }
    }
}
