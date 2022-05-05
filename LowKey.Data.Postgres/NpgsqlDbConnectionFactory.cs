using LowKey.Data.Model;
using Npgsql;
using System.Data.Common;

namespace LowKey.Data.Postgres
{
    public class NpgsqlDbConnectionFactory : IClientFactory<DbConnection>, IClientFactory<NpgsqlConnection>
    {
        private readonly NpgsqlConnectionStringBuilder _connStrBuilder;

        public NpgsqlDbConnectionFactory(NpgsqlConnectionStringBuilder connStrBuilder)
        {
            _connStrBuilder = connStrBuilder;
        }

        Task<DbConnection> IClientFactory<DbConnection>.CreateForStore(DataStore dataStore, Tenant tenant, CancellationToken cancel)
        {
            ConfigureConnectionStringBuilder(dataStore, tenant);

            DbConnection connection = new NpgsqlConnection(_connStrBuilder.ConnectionString);
            return Task.FromResult(connection);
        }

        Task<NpgsqlConnection> IClientFactory<NpgsqlConnection>.CreateForStore(DataStore dataStore, Tenant tenant, CancellationToken cancel)
        {
            ConfigureConnectionStringBuilder(dataStore, tenant);

            var connection = new NpgsqlConnection(_connStrBuilder.ConnectionString);
            return Task.FromResult(connection);
        }

        private void ConfigureConnectionStringBuilder(DataStore dataStore, Tenant tenant)
        {
            _connStrBuilder.Host = tenant.Server;
            _connStrBuilder.Database = dataStore.Name;

            if (tenant.Port.HasValue)
            {
                _connStrBuilder.Port = tenant.Port.Value;
            }
        }
    }
}