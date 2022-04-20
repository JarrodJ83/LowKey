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
        IClientFactory<SqlConnection> _sqlConnFactory;

        public SqlDbConnectionFactoryTests()
        {
            var sqlDbConnectionFactory = new SqlDbConnectionFactory(new SqlConnectionStringBuilder());
            _dbConnFactory = sqlDbConnectionFactory;
            _sqlConnFactory = sqlDbConnectionFactory;
        }

        [Theory, AutoData]
        public async Task DbConnectionIsForCorrectDatabase(DataStore dataStore, Tenant tenant)
        {
            DbConnection conn = await _dbConnFactory.CreateForStore(dataStore, tenant);

            Assert.NotNull(conn);
            Assert.Equal(dataStore.Name, conn.Database);

            var connectionStringBuilder = new SqlConnectionStringBuilder(conn.ConnectionString);

            string[] datasourceParts = connectionStringBuilder.DataSource.Split(":");
            string server = datasourceParts[0];
            int port = int.Parse(datasourceParts[1]);

            Assert.Equal(tenant.Server, server);
            Assert.Equal(tenant.Port, port);
        }

        [Theory, AutoData]
        public async Task SqlConnectionIsForCorrectDatabase(DataStore dataStore, Tenant tenant)
        {
            SqlConnection conn = await _sqlConnFactory.CreateForStore(dataStore, tenant);

            Assert.NotNull(conn);
            Assert.Equal(dataStore.Name, conn.Database);

            var connectionStringBuilder = new SqlConnectionStringBuilder(conn.ConnectionString);

            string[] datasourceParts = connectionStringBuilder.DataSource.Split(":");
            string server = datasourceParts[0];
            int port = int.Parse(datasourceParts[1]);

            Assert.Equal(tenant.Server, server);
            Assert.Equal(tenant.Port, port);
        }
    }
}
