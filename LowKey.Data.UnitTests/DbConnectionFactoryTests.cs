using AutoFixture.Xunit2;
using LowKey.Data.Sql;
using System.Data.Common;
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
    }
}
