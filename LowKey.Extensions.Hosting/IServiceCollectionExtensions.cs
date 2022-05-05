using LowKey.Data;
using LowKey.Data.Extensions.Hosting;
using LowKey.Data.MultiTenancy;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLowKeyData(this IServiceCollection services, Action<LowKeyConfiguration> configuration)
        {
            services.AddSingleton<IDataStoreTenantResolver, DataStoreTenantResolver>();
            services.AddSingleton<IClientFactory, LowKeyClientFactory>();

            var config = new LowKeyConfiguration();

            configuration(config);

            services.AddSingleton(config);
            services.AddSingleton(config.DataStoreTanantResolverRegistry);
            services.AddSingleton(config.DataStoreClientFactoryRegistry);
            services.AddSingleton(config.DataStoreRegistry);

            return services;
        }

    }
}
