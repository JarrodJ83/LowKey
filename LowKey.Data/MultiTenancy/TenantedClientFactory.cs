﻿using LowKey.Data.Model;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data.MultiTenancy
{
    public class TenantedClientFactory : ITenantedClientFactory
    {
        private readonly DataStoreClientFactoryRegistry _dataStoreClientRegistry;
        private readonly DataStoreRegistry _dataStoreRegistry;

        public TenantedClientFactory(DataStoreClientFactoryRegistry dataStoreClientRegistry, DataStoreRegistry dataStoreRegistry)
        {
            _dataStoreClientRegistry = dataStoreClientRegistry;
            _dataStoreRegistry = dataStoreRegistry;
        }

        public Task<TClient> GetClientFor<TClient>(DataStoreId dataStoreId, Tenant tenant, CancellationToken cancellation = default)
        {
            IClientFactory<TClient> clientFactory = _dataStoreClientRegistry.ResolveClientFactory<TClient>(dataStoreId);
            DataStore dataStore = _dataStoreRegistry.GetDataStore(dataStoreId);
            return clientFactory.CreateForStore(dataStore, tenant, cancellation);
        }
    }
}
