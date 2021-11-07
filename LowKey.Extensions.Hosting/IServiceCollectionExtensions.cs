using LowKey.Data;
using LowKey.Data.Diagnostics;
using LowKey.Data.Transactions;
using LowKey.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLowKeyData(this IServiceCollection services, Db db, LowKeyDataOptions? lowKeyDataOptions = default) =>
            AddLowKeyData(services, new Db[] { db }, lowKeyDataOptions);

        public static IServiceCollection AddLowKeyData(this IServiceCollection services, Db[] dbs, LowKeyDataOptions? lowKeyDataOptions = default)
        {
            foreach(var db in dbs)
            {
                services.AddSingleton(db.GetType(), db);
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
