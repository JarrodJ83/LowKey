using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace LowKey.Data.Sql
{
    public class SqlDbConnectionFactory : IClientFactory<DbConnection>
    {
        private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder;

        public SqlDbConnectionFactory(SqlConnectionStringBuilder sqlConnectionStringBuilder)
        {
            _sqlConnectionStringBuilder = sqlConnectionStringBuilder;
        }

        public Task<DbConnection> CreateForStore(Db db)
        {
            _sqlConnectionStringBuilder.InitialCatalog = db.Name;
            _sqlConnectionStringBuilder.DataSource = db.Server;
                
            if(db.Port.HasValue)
            {
                _sqlConnectionStringBuilder.DataSource += $":{db.Port}";
            }

            DbConnection connection = new SqlConnection(_sqlConnectionStringBuilder.ConnectionString);
            return Task.FromResult(connection);
        }
    }
}
