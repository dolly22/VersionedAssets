using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.CdnAssets
{
    public class AssetInfoFeature : IAssetInfoFeature
    {
        public bool HashMatched { get; set; }
    }
}
