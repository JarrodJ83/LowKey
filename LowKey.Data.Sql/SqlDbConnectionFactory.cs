using LowKey.Data.Model;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace LowKey.Data.Sql
{
    public class SqlDbConnectionFactory : IClientFactory<DbConnection>
    {
        private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder;

        public SqlDbConnectionFactory(SqlConnectionStringBuilder sqlConnectionStringBuilder)
        {
            _sqlConnectionStringBuilder = sqlConnectionStringBuilder;
        }

        public Task<DbConnection> CreateForStore(DataStore dataStore, Tenant tenant)
        {
            _sqlConnectionStringBuilder.InitialCatalog = dataStore.Name;
            _sqlConnectionStringBuilder.DataSource = tenant.Server;
                
            if(tenant.Port.HasValue)
            {
                _sqlConnectionStringBuilder.DataSource += $":{tenant.Port}";
            }

            DbConnection connection = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString);
            return Task.FromResult(connection);
        }
    }
}
