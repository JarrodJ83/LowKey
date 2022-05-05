using LowKey.Data;
using LowKey.Data.Postgres;
using LowKey.Data.Extensions.Hosting;
using Npgsql;
using System.Data.Common;

namespace Microsoft.Extensions.Hosting
{
    public static class NpsqlDataStoreConfigExtensions
    {
        public static LowKeyDataStoreConfig WithSqlServer(this LowKeyDataStoreConfig config, NpgsqlConnectionStringBuilder connectionStringBuilder)
        {
            config.UseClientFactory(() => (IClientFactory<DbConnection>)new NpgsqlDbConnectionFactory(connectionStringBuilder));
            config.UseClientFactory(() => (IClientFactory<NpgsqlConnection>)new NpgsqlDbConnectionFactory(connectionStringBuilder));

            return config;
        }
    }
}
