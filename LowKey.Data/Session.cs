using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public partial class Session<TClient> : IQuerySession<TClient>
    {
        private readonly ITenantedQuerySession<TClient> _tenantedQuerySession;
        private readonly DataStoreTanantResolverRegistry _dataStoreTanantResolverRegistry;

        public Session(ITenantedQuerySession<TClient> tenantedQuerySession, DataStoreTanantResolverRegistry dataStoreTanantResolverRegistry)
        {
            _tenantedQuerySession = tenantedQuerySession;
            _dataStoreTanantResolverRegistry = dataStoreTanantResolverRegistry;
        }

        public async Task<TResult> Execute<TResult>(DataStoreId dataStoreId, Func<TClient, Task<TResult>> query, CancellationToken cancellation = default)
        {
            ITenantResolver tenantResolver = await _dataStoreTanantResolverRegistry.GetTenantResolverFor(dataStoreId, cancellation);
            Tenant tenant = await tenantResolver.Resolve(dataStoreId);
            return await _tenantedQuerySession.Execute(dataStoreId, tenant, query, cancellation);
        }
    }
}
