using LowKey.Data;
using LowKey.Data.Diagnostics;
using LowKey.Data.Transactions;
using LowKey.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLowKeyData(this IServiceCollection services, Action<LowKeyConfiguration> configuration, LowKeyDataOptions lowKeyDataOptions = default)
        {
            services.AddScoped(typeof(ITenantedQuerySession<>), typeof(TenantedSession<>));
            services.AddScoped(typeof(ITenantedCommandSession<>), typeof(TenantedSession<>));

            services.AddScoped(typeof(IQuerySession<>), typeof(QuerySession<>));
            services.AddScoped(typeof(ICommandSession<>), typeof(CommandSession<>));

            services.AddSingleton<IDataStoreTenantResolver, DataStoreTenantResolver>();

            if (lowKeyDataOptions?.EnableDiagnosticActivities == true)
            {
                services.Decorate(typeof(ITenantedQuerySession<>), typeof(ActivityTenantedQuerySession<>));
                services.Decorate(typeof(ITenantedCommandSession<>), typeof(ActivityTenantedCommandSession<>));
            }

            if (lowKeyDataOptions?.CommandOptions?.TransactionSettings is not null)
            {
                services.AddSingleton(lowKeyDataOptions.CommandOptions.TransactionSettings);
                services.Decorate(typeof(ITenantedCommandSession<>), typeof(TransactionalTenantedCommandSession<>));
            }

            var config = new LowKeyConfiguration();

            configuration(config);

            services.AddSingleton(config);
            services.AddSingleton(config.DataStoreTanantResolverRegistry);
            services.AddSingleton(config.DataStoreClientFactoryRegistry);

            return services;
        }

    }
}
