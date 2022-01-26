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
        private readonly IQuerySession<DbConnection> _qrySession;
        private readonly ICommandSession<DbConnection> _cmdSession;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndexModel(ILogger<IndexModel> logger, IQuerySession<DbConnection> qrySession, ICommandSession<DbConnection> cmdSession, IHttpContextAccessor httpContextAccessor)
        {
            _qrySession = qrySession;
            _cmdSession = cmdSession;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnGet()
        {
            var dataStoreId = new DataStoreId(_httpContextAccessor.HttpContext.Request.Query.ContainsKey("tenant") ? "sql-multi-tenant" : "sql");
            await _cmdSession.Execute<DbConnection>(dataStoreId, conn => conn.ExecuteAsync("select 1"));
            var result = await _qrySession.Execute(dataStoreId, conn => conn.QueryAsync("select 1"));
        }
    }
}
