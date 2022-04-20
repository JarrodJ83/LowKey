using LowKey.Data;
using LowKey.Data.Sql;
using LowKey.Extensions.Hosting;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting
{
    public static class LowKeyDataStoreConfigExtensions
    {
        public static LowKeyDataStoreConfig WithSqlServer(this LowKeyDataStoreConfig config, SqlConnectionStringBuilder connectionStringBuilder)
        {
            config.UseClientFactory(cancel => Task.FromResult((IClientFactory<DbConnection>)new SqlDbConnectionFactory(connectionStringBuilder)));
            config.UseClientFactory(cancel => Task.FromResult((IClientFactory<SqlConnection>)new SqlDbConnectionFactory(connectionStringBuilder)));

            return config;
        }
    }
}
