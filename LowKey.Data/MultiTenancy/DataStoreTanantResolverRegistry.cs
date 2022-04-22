using LowKey.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class DataStoreTanantResolverRegistry
    {
        private Dictionary<DataStoreId, Func<ITenantResolver>> _tenantResolverRegistry;
        private Dictionary<DataStoreId, Func<ITenantIdResolver>> _tenantIdResolverRegistry;

        public DataStoreTanantResolverRegistry()
        {
            _tenantResolverRegistry = new();
            _tenantIdResolverRegistry = new();
        }

        public void RegisterTenantResolverFor(DataStoreId dataStoreId, 
            Func<ITenantResolver> tenantResolverFactory, 
            Func<ITenantIdResolver> tenantIdResolverFactory)
        {
            _tenantResolverRegistry.Add(dataStoreId, tenantResolverFactory);
            _tenantIdResolverRegistry.Add(dataStoreId, tenantIdResolverFactory);
        }
        public ITenantResolver GetTenantResolverFor(DataStoreId dataStoreId)
        {
            if (_tenantResolverRegistry.TryGetValue(dataStoreId, out var tenantResolverFactory))
            {
                return tenantResolverFactory();
            }

            throw new InvalidOperationException($"No {nameof(ITenantResolver)} registered for \"{dataStoreId.Value}\" DataStore");
        }

        public ITenantIdResolver GetTenantIdResolverFor(DataStoreId dataStoreId, CancellationToken cancel = default)
        {
            if (_tenantIdResolverRegistry.TryGetValue(dataStoreId, out var tenantIdResolverFactory))
            {
                return tenantIdResolverFactory();
            }

            throw new InvalidOperationException($"No {nameof(ITenantIdResolver)} registered for \"{dataStoreId.Value}\" DataStore");
        }
    }
}