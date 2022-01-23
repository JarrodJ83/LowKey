﻿using Dapper;
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

        public IndexModel(ILogger<IndexModel> logger, IQuerySession<DbConnection> qrySession, ICommandSession<DbConnection> cmdSession)
        {
            _qrySession = qrySession;
            _cmdSession = cmdSession;
            _logger = logger;
        }

        public async Task OnGet()
        {
            var dataStoreId = new DataStoreId("master");
            await _cmdSession.Execute<DbConnection>(dataStoreId, conn => conn.ExecuteAsync("select 1"));
            var result = await _qrySession.Execute(dataStoreId, conn => conn.QueryAsync("select 1"));
        }
    }
}
