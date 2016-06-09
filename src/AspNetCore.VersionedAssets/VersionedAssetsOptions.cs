using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.VersionedAssets
{
    public class VersionedAssetsOptions
    {
        public VersionedAssetsOptions()
        {
            this.Caching = new VersionedAssetsCaching();
        }

        /// <summary>
        /// Enable versioning (when disabled tag helper do not alter url attributes)
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Server side - path match versioned assets middleware handles asset requests. Defaults to '/static'
        /// </summary>
        public string PathMatch { get; set; } = "/static";

        /// <summary>
        /// Client side - url prefix to use when generationg taghelper urls. Defaults to '/static'
        /// </summary>
        public string UrlPrefix { get; set; } = "/static";

        /// <summary>
        /// Global version to use
        /// </summary>
        public string GlobalVersion { get; set; }

        /// <summary>
        /// Assets caching options
        /// </summary>
        public VersionedAssetsCaching Caching { get; set; }
    }

    public class VersionedAssetsCaching
    {
        /// <summary>
        /// Cache is public
        /// </summary>
        public bool Public { get; set; } = true;

        /// <summary>
        /// Cache max age
        /// </summary>
        public TimeSpan MaxAge { get; set; } = TimeSpan.FromDays(365);
    }
}
