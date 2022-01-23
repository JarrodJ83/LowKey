using LowKey.Data;
using LowKey.Data.Model;
using LowKey.Data.Sql;
using LowKey.Extensions.Hosting;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting
{
    public static class LowKeyConfigurationExtensions
    {
        public static LowKeyConfiguration WithSqlServer(this LowKeyConfiguration config, DataStoreId dataStoreId, SqlConnectionStringBuilder connectionStringBuilder)
        {
            config.DataStoreClientFactoryRegistry.RegisterClientFor(dataStoreId, cancel => Task.FromResult((IClientFactory<DbConnection>)new SqlDbConnectionFactory(connectionStringBuilder)));

            return config;
        }
    }
}
