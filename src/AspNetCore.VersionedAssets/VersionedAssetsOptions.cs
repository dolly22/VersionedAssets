using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.VersionedAssets
{
    public class VersionedAssetsOptions
    {
        public bool IsEnabled { get; set; } = true;

        public string UrlPrefix { get; set; } = "/static";
    }
}
