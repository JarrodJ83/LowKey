using AutoFixture.Xunit2;
using LowKey.Data.Sql;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class DbConnectionFactoryTests
    {
        IClientFactory<DbConnection> _dbConnFactory;
        public DbConnectionFactoryTests()
        {
            _dbConnFactory = new SqlDbConnectionFactory(new System.Data.SqlClient.SqlConnectionStringBuilder());
        }

        [Theory, AutoData]
        public async Task CreateDbConnection(Db testDb)
        {
            var conn = await _dbConnFactory.CreateForStore(testDb);

            Assert.NotNull(conn);
        }

        [Theory, AutoData]
        public async Task ConnectionIsForCorrectDatabase(Db testDb)
        {
            var conn = await _dbConnFactory.CreateForStore(testDb);

            Assert.Equal(conn.Database, testDb.Name);
            var connectionStringBuilder = new SqlConnectionStringBuilder(conn.ConnectionString);

            var datasourceParts = connectionStringBuilder.DataSource.Split(":");
            var server = datasourceParts[0];
            var port = datasourceParts[1];

            Assert.Equal(server, testDb.Server);
            Assert.Equal(int.Parse(port), testDb.Port);
        }
    }
}
