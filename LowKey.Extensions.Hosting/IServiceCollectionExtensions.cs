using LowKey.Data;
using LowKey.Data.Diagnostics;
using LowKey.Data.Transactions;
using LowKey.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLowKeyData(this IServiceCollection services, Tenant tenant, LowKeyDataOptions? lowKeyDataOptions = default) =>
            AddLowKeyData(services, new Tenant[] { tenant }, lowKeyDataOptions);

        public static IServiceCollection AddLowKeyData(this IServiceCollection services, Tenant[] tenants, LowKeyDataOptions? lowKeyDataOptions = default)
        {
            foreach(var tenant in tenants)
            {
                services.AddSingleton(tenant.GetType(), tenant);
            }
            
            services.AddScoped(typeof(ITenantedQuerySession<>), typeof(TenantedSession<>));
            services.AddScoped(typeof(ITenantedCommandSession<>), typeof(TenantedSession<>));

            if(lowKeyDataOptions?.EnableDiagnosticActivities == true)
            {
                services.Decorate(typeof(ITenantedQuerySession<>), typeof(ActivityTenantedQuerySession<>));
                services.Decorate(typeof(ITenantedCommandSession<>), typeof(ActivityTenantedCommandSession<>));
            }

            if(lowKeyDataOptions?.CommandOptions?.TransactionSettings is not null)
            {
                services.AddSingleton(lowKeyDataOptions.CommandOptions.TransactionSettings);
                services.Decorate(typeof(ITenantedCommandSession<>), typeof(TransactionalTenantedCommandSession<>));
            }

            return services;
        }
    }
}
