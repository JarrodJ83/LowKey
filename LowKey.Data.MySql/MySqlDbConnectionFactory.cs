using LowKey.Data.Model;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace LowKey.Data.Sql
{
    public class MySqlDbConnectionFactory : IClientFactory<DbConnection>, IClientFactory<MySqlConnection>
    {
        private readonly MySqlBaseConnectionStringBuilder _mySqlConnectionStringBuilder;

        public MySqlDbConnectionFactory(MySqlBaseConnectionStringBuilder mySqlConnectionStringBuilder)
        {
            _mySqlConnectionStringBuilder = mySqlConnectionStringBuilder;
        }

        Task<DbConnection> IClientFactory<DbConnection>.CreateForStore(DataStore dataStore, Tenant tenant, CancellationToken cancel)
        {
            ConfigureConnectionStringBuilder(dataStore, tenant);

            DbConnection connection = new MySqlConnection(_mySqlConnectionStringBuilder.ConnectionString);
            return Task.FromResult(connection);
        }

        Task<MySqlConnection> IClientFactory<MySqlConnection>.CreateForStore(DataStore dataStore, Tenant tenant, CancellationToken cancel)
        {
            ConfigureConnectionStringBuilder(dataStore, tenant);

            var connection = new MySqlConnection(_mySqlConnectionStringBuilder.ConnectionString);
            return Task.FromResult(connection);
        }

        private void ConfigureConnectionStringBuilder(DataStore dataStore, Tenant tenant)
        {
            _mySqlConnectionStringBuilder.Database = dataStore.Name;
            _mySqlConnectionStringBuilder.Server = tenant.Server;

            if (tenant.Port.HasValue)
            {
                _mySqlConnectionStringBuilder.Port = (uint)tenant.Port.Value;
            }
        }
    }
}
