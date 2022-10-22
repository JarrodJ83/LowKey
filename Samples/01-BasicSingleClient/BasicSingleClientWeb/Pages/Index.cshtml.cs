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

        public string ConnectionString { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IClientFactory dbConnFactory)
        {
            _logger = logger;
            _dbConnFactory = dbConnFactory;
        }

        public async void OnGet()
        {
            var client = await _dbConnFactory.Create<DbConnection>(new DataStoreId("sql_master"));

            ConnectionString = client.ConnectionString;
        }
    }
}