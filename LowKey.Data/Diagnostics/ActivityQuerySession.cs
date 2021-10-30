using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data.Diagnostics
{
    public class ActivityQuerySession<TClient> : IQuerySession<TClient>
    {
        private readonly IQuerySession<TClient> _querySession;
        private const string Operation = "Query";
        
        public ActivityQuerySession(IQuerySession<TClient> querySession)
        {
            _querySession = querySession;
        }

        public async Task<TResult> Execute<TResult>(Db db, Func<TClient, Task<TResult>> query, CancellationToken cancellation = default)
        {
            using Activity? activity = ActivitySources.SessionActivity.StartActivity($"{nameof(Execute)} {typeof(TClient).FullName} {Operation}", ActivityKind.Client);

            activity.SetLowKeyActivityTags(db, typeof(TClient), Operation);

            // Don't return a task here or it will not be executed inside of the ActivitySource!
            return await _querySession.Execute(db, query, cancellation);
        }
    }
}
