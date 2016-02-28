using AspNetCore.VersionedAssets;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class VersionedAssetServiceCollectionExtensions
    {
        public static void AddVersionedAssets(this IServiceCollection services, Action<VersionedAssetsOptions> setupAction = null)
        {
            var options = new VersionedAssetsOptions();
            if (setupAction != null)
                setupAction.Invoke(options);

            services.Add(ServiceDescriptor.Instance<Func<VersionedAssetsOptions>>(() => options));
        }
    }
}
