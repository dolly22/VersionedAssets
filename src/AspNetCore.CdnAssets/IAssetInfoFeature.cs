using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.CdnAssets
{
    public interface IAssetInfoFeature
    {
        bool HashMatched { get; set; }
    }
}
