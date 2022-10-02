using LowKey.Data;
using LowKey.Data.Sql;
using LowKey.Data.Extensions.Hosting;
using System.Data.Common;
using System.Data;
using MySql.Data.MySqlClient;

namespace Microsoft.Extensions.Hosting
{
    public static class MySqlDataStoreConfigExtensions
    {
        public static LowKeyDataStoreConfig WithMySql(this LowKeyDataStoreConfig config, MySqlConnectionStringBuilder connectionStringBuilder)
        {
            config.UseClientFactory(() => (IClientFactory<IDbConnection>)new MySqlDbConnectionFactory(connectionStringBuilder));
            config.UseClientFactory(() => (IClientFactory<DbConnection>)new MySqlDbConnectionFactory(connectionStringBuilder));
            config.UseClientFactory(() => (IClientFactory<MySqlConnection>)new MySqlDbConnectionFactory(connectionStringBuilder));

            return config;
        }
    }
}
