using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.VersionedAssets
{
    public interface IAssetInfoFeature
    {
        /// <summary>
        /// Url hash matched server side hash?
        /// </summary>
        bool UrlHashMatched { get; }
    }
}
