using LowKey.Data;
using LowKey.Data.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Common;

namespace BasicSingleClientWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IClientFactory _dbConnFactory;

        public string SqlConnectionString { get; set; }
        public string PgConnectionString { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IClientFactory dbConnFactory)
        {
            _logger = logger;
            _dbConnFactory = dbConnFactory;
        }

        public async void OnGet()
        {
            var sqlCclient = await _dbConnFactory.Create<DbConnection>(new DataStoreId("sql_master"));

            SqlConnectionString = sqlCclient.ConnectionString;

            var pgClient = await _dbConnFactory.Create<DbConnection>(new DataStoreId("pg_main"));

            PgConnectionString = pgClient.ConnectionString; 
        }
    }
}