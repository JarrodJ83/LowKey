using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class DataStoreTanantResolverRegistry
    {
        private Dictionary<DataStoreId, Func<CancellationToken, Task<ITenantResolver>>> _tenantResolverRegistry;
        private Dictionary<DataStoreId, Func<CancellationToken, Task<ITenantIdResolver>>> _tenantIdResolverRegistry;

        public DataStoreTanantResolverRegistry()
        {
            _tenantResolverRegistry = new();
            _tenantIdResolverRegistry = new();
        }

        public void RegisterTenantResolverFor(DataStoreId dataStoreId, Func<CancellationToken, Task<ITenantResolver>> tenantResolverFactory, Func<CancellationToken, Task<ITenantIdResolver>> tenantIdResolverFactory)
        {
            _tenantResolverRegistry.Add(dataStoreId, tenantResolverFactory);
            _tenantIdResolverRegistry.Add(dataStoreId, tenantIdResolverFactory);
        }
        public Task<ITenantResolver> GetTenantResolverFor(DataStoreId dataStoreId, CancellationToken cancel = default)
        {
            if (_tenantResolverRegistry.TryGetValue(dataStoreId, out var tenantResolverFactory))
            {
                return tenantResolverFactory(cancel);
            }

            throw new InvalidOperationException($"No {nameof(ITenantResolver)} registered for \"{dataStoreId.Value}\" DataStore");
        }

        public Task<ITenantIdResolver> GetTenantIdResolverFor(DataStoreId dataStoreId, CancellationToken cancel = default)
        {
            if (_tenantIdResolverRegistry.TryGetValue(dataStoreId, out var tenantIdResolverFactory))
            {
                return tenantIdResolverFactory(cancel);
            }

            throw new InvalidOperationException($"No {nameof(ITenantIdResolver)} registered for \"{dataStoreId.Value}\" DataStore");
        }
    }
}