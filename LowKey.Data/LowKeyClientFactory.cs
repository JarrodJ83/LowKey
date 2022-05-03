using LowKey.Data.Model;
using LowKey.Data.MultiTenancy;
using System.Threading;
using System.Threading.Tasks;

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
            Tenant tenant = await _dataStoreTenantResolver.Resolve(dataStoreId, cancel);

            return await GetClientFor<TClient>(dataStoreId, tenant, cancel);
        }

        Task<TClient> GetClientFor<TClient>(DataStoreId dataStoreId, Tenant tenant, CancellationToken cancellation = default)
        {
            IClientFactory<TClient> clientFactory = _dataStoreClientRegistry.ResolveClientFactory<TClient>(dataStoreId);
            DataStore dataStore = _dataStoreRegistry.GetDataStore(dataStoreId);
            return clientFactory.CreateForStore(dataStore, tenant, cancellation);
        }
    }
}
