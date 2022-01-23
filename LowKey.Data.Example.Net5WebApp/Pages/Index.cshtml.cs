using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Threading.Tasks;

namespace LowKey.Data.Example.Net5WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IQuerySession<DbConnection> _connSession;

        public IndexModel(ILogger<IndexModel> logger, IQuerySession<DbConnection> connSession)
        {
            _connSession = connSession;
            _logger = logger;
        }

        public async Task OnGet()
        {
            var result = await _connSession.Execute(new DataStoreId("master"), conn => conn.QueryAsync("select 1"));
        }
    }
}
