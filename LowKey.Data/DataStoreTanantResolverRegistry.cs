using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class DataStoreTanantResolverRegistry
    {
        private Dictionary<DataStoreId, Func<CancellationToken, Task<ITenantResolver>>> _tenantResolverRegistry;

        public DataStoreTanantResolverRegistry()
        {
            _tenantResolverRegistry = new();
        }

        public void RegisterTenantResolverFor(DataStoreId dataStoreId, Func<CancellationToken, Task<ITenantResolver>> tenantResolverFactory) =>
            _tenantResolverRegistry.Add(dataStoreId, tenantResolverFactory);

        public Task<ITenantResolver> GetTenantResolverFor(DataStoreId dataStoreId, CancellationToken cancel = default)
        {
            if (_tenantResolverRegistry.TryGetValue(dataStoreId, out var tenantResolverFactory))
            {
                return tenantResolverFactory(cancel);
            }

            throw new InvalidOperationException($"No {nameof(ITenantResolver)} registered for \"{dataStoreId.Name}\" DataStore");
        }
    }
}