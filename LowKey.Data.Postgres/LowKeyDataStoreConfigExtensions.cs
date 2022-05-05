using LowKey.Data;
using LowKey.Data.Postgres;
using LowKey.Data.Extensions.Hosting;
using Npgsql;
using System.Data.Common;
using System.Data;

namespace Microsoft.Extensions.Hosting
{
    public static class NpsqlDataStoreConfigExtensions
    {
        public static LowKeyDataStoreConfig WithPostgres(this LowKeyDataStoreConfig config, NpgsqlConnectionStringBuilder connectionStringBuilder)
        {
            config.UseClientFactory(() => (IClientFactory<IDbConnection>)new NpgsqlDbConnectionFactory(connectionStringBuilder));
            config.UseClientFactory(() => (IClientFactory<DbConnection>)new NpgsqlDbConnectionFactory(connectionStringBuilder));
            config.UseClientFactory(() => (IClientFactory<NpgsqlConnection>)new NpgsqlDbConnectionFactory(connectionStringBuilder));

            return config;
        }
    }
}
