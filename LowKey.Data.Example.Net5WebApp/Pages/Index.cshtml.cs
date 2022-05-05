using Dapper;
using LowKey.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace LowKey.Data.Example.Net5WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndexModel(ILogger<IndexModel> logger, IClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnGet()
        {
            bool useTenantedStore = _httpContextAccessor.HttpContext.Request.Query.ContainsKey("tenant");

            await RunSqlQuery(useTenantedStore);
            await RunNpgsqlQuery(useTenantedStore);
        }

        async Task RunSqlQuery(bool useTenantedStore)
        {
            var dataStoreId = new DataStoreId(useTenantedStore ? DataStores.SqlMultiTenant : DataStores.Sql);
            DbConnection dbConnClient = await _clientFactory.Create<DbConnection>(dataStoreId);

            await dbConnClient.QueryFirstAsync<int>("select 1");


            SqlConnection sqlConnClient = await _clientFactory.Create<SqlConnection>(dataStoreId);

            await sqlConnClient.QueryFirstAsync<int>("select 1");
        }

        async Task RunNpgsqlQuery(bool useTenantedStore)
        {
            var dataStoreId = new DataStoreId(useTenantedStore ? DataStores.PostgresMultiTenant : DataStores.Postgres);

            DbConnection dbConnClient = await _clientFactory.Create<DbConnection>(dataStoreId);

            await dbConnClient.QueryFirstAsync<int>("select 1");


            NpgsqlConnection npgsqlConnClient = await _clientFactory.Create<NpgsqlConnection>(dataStoreId);

            await npgsqlConnClient.QueryFirstAsync<int>("select 1");
        }
    }
}
