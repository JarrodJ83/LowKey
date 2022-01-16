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
        private readonly ICommandSession<DbConnection> _connSession;
        private readonly CoreDb _coreDb;

        public IndexModel(ILogger<IndexModel> logger, ICommandSession<DbConnection> connSession, CoreDb coreDb)
        {
            _connSession = connSession;
            _logger = logger;
            _coreDb = coreDb;
        }

        public async Task OnGet()
        {
            await _connSession.Execute(new DataStoreId("coredb"), _coreDb, conn => conn.QueryAsync("select 1"));
        }
    }
}
