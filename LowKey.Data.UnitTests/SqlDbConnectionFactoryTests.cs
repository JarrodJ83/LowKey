using AutoFixture.Xunit2;
using LowKey.Data.Model;
using LowKey.Data.Sql;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class SqlDbConnectionFactoryTests
    {
        IClientFactory<DbConnection> _dbConnFactory;
        public SqlDbConnectionFactoryTests()
        {
            _dbConnFactory = new SqlDbConnectionFactory(new SqlConnectionStringBuilder());
        }

        [Theory, AutoData]
        public async Task ConnectionIsForCorrectDatabase(DataStoreId dataStoreId, Tenant tenant)
        {
            DbConnection conn = await _dbConnFactory.CreateForStore(dataStoreId, tenant);

            Assert.NotNull(conn);
            Assert.Equal(dataStoreId.Value, conn.Database);

            var connectionStringBuilder = new SqlConnectionStringBuilder(conn.ConnectionString);

            string[] datasourceParts = connectionStringBuilder.DataSource.Split(":");
            string server = datasourceParts[0];
            int port = int.Parse(datasourceParts[1]);

            Assert.Equal(tenant.Server, server);
            Assert.Equal(tenant.Port, port);
        }
    }
}
