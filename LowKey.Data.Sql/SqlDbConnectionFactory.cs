using LowKey.Data.Model;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data.Sql
{
    public class SqlDbConnectionFactory : IClientFactory<DbConnection>, IClientFactory<SqlConnection>
    {
        private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder;

        public SqlDbConnectionFactory(SqlConnectionStringBuilder sqlConnectionStringBuilder)
        {
            _sqlConnectionStringBuilder = sqlConnectionStringBuilder;
        }

        Task<DbConnection> IClientFactory<DbConnection>.CreateForStore(DataStore dataStore, Tenant tenant, CancellationToken cancel)
        {
            ConfigureConnectionStringBuilder(dataStore, tenant);

            DbConnection connection = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString);
            return Task.FromResult(connection);
        }

        Task<SqlConnection> IClientFactory<SqlConnection>.CreateForStore(DataStore dataStore, Tenant tenant, CancellationToken cancel)
        {
            ConfigureConnectionStringBuilder(dataStore, tenant);

            var connection = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString);
            return Task.FromResult(connection);
        }

        private void ConfigureConnectionStringBuilder(DataStore dataStore, Tenant tenant)
        {
            _sqlConnectionStringBuilder.InitialCatalog = dataStore.Name;
            _sqlConnectionStringBuilder.DataSource = tenant.Server;

            if (tenant.Port.HasValue)
            {
                _sqlConnectionStringBuilder.DataSource += $":{tenant.Port}";
            }
        }
    }
}
