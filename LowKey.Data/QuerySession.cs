using LowKey.Data.Model;
using LowKey.Data.MultiTenancy;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class QuerySession<TClient> : IQuerySession<TClient>
    {
        private readonly ITenantedQuerySession<TClient> _tenantedQuerySession;
        private readonly IDataStoreTenantResolver _dataStoreTenantResolver;

        public QuerySession(ITenantedQuerySession<TClient> tenantedQuerySession, IDataStoreTenantResolver dataStoreTenantResolver)
        {
            _tenantedQuerySession = tenantedQuerySession;
            _dataStoreTenantResolver = dataStoreTenantResolver;
        }

        public async Task<TResult> Execute<TResult>(DataStoreId dataStoreId, Func<TClient, Task<TResult>> query, CancellationToken cancellation = default)
        {
            Tenant tenant = await _dataStoreTenantResolver.Resolve(dataStoreId, cancellation);
            return await _tenantedQuerySession.Execute(dataStoreId, tenant, query, cancellation);
        }       
    }
}
