using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.VersionedAssets
{
    public class AssetInfoFeature : IAssetInfoFeature
    {
        public static readonly IAssetInfoFeature Matched = new AssetInfoFeature { UrlHashMatched = true };
        public static readonly IAssetInfoFeature NotMatched = new AssetInfoFeature { UrlHashMatched = false };

        public bool UrlHashMatched { get; set; }
    }
}
