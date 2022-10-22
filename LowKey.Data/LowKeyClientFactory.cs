using LowKey.Data.Model;
using LowKey.Data.MultiTenancy;

namespace LowKey.Data
{
    public class LowKeyClientFactory : IClientFactory
    {
        private readonly IDataStoreTenantResolver _dataStoreTenantResolver;
        private readonly DataStoreClientFactoryRegistry _dataStoreClientRegistry;
        private readonly DataStoreRegistry _dataStoreRegistry;
        public LowKeyClientFactory(IDataStoreTenantResolver dataStoreTenantResolver, DataStoreClientFactoryRegistry dataStoreClientRegistry, DataStoreRegistry dataStoreRegistry)
        {
            _dataStoreTenantResolver = dataStoreTenantResolver;
            _dataStoreClientRegistry = dataStoreClientRegistry;
            _dataStoreRegistry = dataStoreRegistry;
        }

        public async Task<TClient> Create<TClient>(DataStoreId dataStoreId, CancellationToken cancel = default)
        {
            DataStore dataStore = _dataStoreRegistry.GetDataStore(dataStoreId);

            Tenant tenant = await _dataStoreTenantResolver.Resolve(dataStoreId, cancel);

            IClientFactory<TClient> clientFactory = _dataStoreClientRegistry.ResolveClientFactory<TClient>(dataStore.Id);

            return await clientFactory.CreateForStore(dataStore, tenant, cancel);
        }
    }
}
