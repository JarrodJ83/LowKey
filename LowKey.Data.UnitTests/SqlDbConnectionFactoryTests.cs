using AutoFixture.Xunit2;
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
        public async Task ConnectionIsForCorrectDatabase(Tenant tenant)
        {
            DbConnection conn = await _dbConnFactory.CreateForStore(tenant);

            Assert.NotNull(conn);
            Assert.Equal(conn.Database, tenant.Name);

            var connectionStringBuilder = new SqlConnectionStringBuilder(conn.ConnectionString);

            string[] datasourceParts = connectionStringBuilder.DataSource.Split(":");
            string server = datasourceParts[0];
            int port = int.Parse(datasourceParts[1]);

            Assert.Equal(server, tenant.Server);
            Assert.Equal(port, tenant.Port);
        }
    }
}
