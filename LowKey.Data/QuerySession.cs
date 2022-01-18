using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class QuerySession<TClient> : IQuerySession<TClient>
    {
        private readonly ITenantedQuerySession<TClient> _tenantedQuerySession;
        private readonly DataStoreTanantResolverRegistry _dataStoreTanantResolverRegistry;

        public QuerySession(ITenantedQuerySession<TClient> tenantedQuerySession, DataStoreTanantResolverRegistry dataStoreTanantResolverRegistry)
        {
            _tenantedQuerySession = tenantedQuerySession;
            _dataStoreTanantResolverRegistry = dataStoreTanantResolverRegistry;
        }

        public async Task<TResult> Execute<TResult>(DataStoreId dataStoreId, Func<TClient, Task<TResult>> query, CancellationToken cancellation = default)
        {
            Tenant tenant = await ResolveTenant(dataStoreId, cancellation);
            return await _tenantedQuerySession.Execute(dataStoreId, tenant, query, cancellation);
        }       

        private async Task<Tenant> ResolveTenant(DataStoreId dataStoreId, CancellationToken cancellation)
        {
            ITenantResolver tenantResolver = await _dataStoreTanantResolverRegistry.GetTenantResolverFor(dataStoreId, cancellation);
            Tenant tenant = await tenantResolver.Resolve(dataStoreId, cancellation);
            return tenant;
        }
    }
}
