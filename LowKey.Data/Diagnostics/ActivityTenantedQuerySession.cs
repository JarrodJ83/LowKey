using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data.Diagnostics
{
    public class ActivityTenantedQuerySession<TClient> : ITenantedQuerySession<TClient>
    {
        private readonly ITenantedQuerySession<TClient> _querySession;
        private const string Operation = "Query";
        
        public ActivityTenantedQuerySession(ITenantedQuerySession<TClient> querySession)
        {
            _querySession = querySession;
        }

        public Task<TResult> Execute<TResult>(DataStoreId dataStoreId, Tenant tenant, Func<TClient, Task<TResult>> query, CancellationToken cancellation = default)
        {
            using Activity? activity = ActivitySources.QuerySessionActivity.StartActivity($"{nameof(Execute)} {typeof(TClient).FullName} {Operation}", ActivityKind.Client);

            activity.SetLowKeyActivityTags(dataStoreId, tenant, typeof(TClient), Operation);

            if (activity?.IsAllDataRequested == true)
            {
                activity?.SetTag(LowKeyDataActivityTags.QueryResultType, typeof(TResult).FullName);
            }

            return _querySession.Execute(dataStoreId, tenant, query, cancellation);
        }
    }
}
