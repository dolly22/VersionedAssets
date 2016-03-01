using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.VersionedAssets
{
    public interface IVersionedAssetsOptions
    {
        bool IsEnabled { get; }

        string UrlPrefix { get; }

        string GlobalVersion { get; }
    }

    public class VersionedAssetsOptions : IVersionedAssetsOptions
    {
        public bool IsEnabled { get; set; } = true;

        public string UrlPrefix { get; set; } = "/static";

        public string GlobalVersion { get; set; }
    }
}
