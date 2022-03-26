using Dapper;
using LowKey.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.Common;
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
            var dataStoreId = new DataStoreId(_httpContextAccessor.HttpContext.Request.Query.ContainsKey("tenant") ? "sql-multi-tenant" : "sql");

            var client = await _clientFactory.Resolve<DbConnection>(dataStoreId);

            var result = await client.QueryFirstAsync<int>("select 1");
        }
    }
}
