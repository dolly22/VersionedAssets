using AspNetCore.VersionedAssets;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class VersionedAssetServiceCollectionExtensions
    {
        public static IServiceCollection AddVersionedAssets(this IServiceCollection services, Action<VersionedAssetsOptions> setupAction = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddOptions();
            services.Configure(setupAction ?? (options => { }));

            return services;
        }
    }
}
