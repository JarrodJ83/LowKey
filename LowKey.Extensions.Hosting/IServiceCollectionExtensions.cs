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
            
            services.AddScoped(typeof(IQuerySession<>), typeof(Session<>));
            services.AddScoped(typeof(ICommandSession<>), typeof(Session<>));

            if(lowKeyDataOptions?.EnableDiagnosticActivities == true)
            {
                services.Decorate(typeof(IQuerySession<>), typeof(ActivityQuerySession<>));
                services.Decorate(typeof(ICommandSession<>), typeof(ActivityCommandSession<>));
            }

            if(lowKeyDataOptions?.CommandOptions?.TransactionSettings is not null)
            {
                services.AddSingleton(lowKeyDataOptions.CommandOptions.TransactionSettings);
                services.Decorate(typeof(ICommandSession<>), typeof(TransactionalCommandSession<>));
            }

            return services;
        }
    }
}
