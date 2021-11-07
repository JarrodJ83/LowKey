using LowKey.Data;
using LowKey.Data.Sql;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Data.SqlClient;

namespace Microsoft.Extensions.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection WithSqlServer(this IServiceCollection services, SqlConnectionStringBuilder connectionStringBuilder)
        {
            services.AddSingleton<IClientFactory<DbConnection>>(provider => new SqlDbConnectionFactory(connectionStringBuilder));

            return services;
        }
    }
}
