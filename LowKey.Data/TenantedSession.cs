using LowKey.Data.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class TenantedSession<TClient> : ITenantedCommandSession<TClient>, ITenantedQuerySession<TClient>
    {
        private readonly DataStoreClientFactoryRegistry _dataStoreClientRegistry;
        private readonly DataStoreRegistry _dataStoreRegistry;

        public TenantedSession(DataStoreClientFactoryRegistry dataStoreClientRegistry, DataStoreRegistry dataStoreRegistry)
        {
            _dataStoreClientRegistry = dataStoreClientRegistry;
            _dataStoreRegistry = dataStoreRegistry;
        }

        public async Task Execute(DataStoreId dataStoreId, Tenant tenant, Func<TClient, Task> command, CancellationToken cancellation = default)
        {
            TClient client = await GetClientFor(dataStoreId, tenant);
            await command(client);
        }

        public async Task<TResult> Execute<TResult>(DataStoreId dataStoreId, Tenant tenant, Func<TClient, Task<TResult>> query, CancellationToken cancellation = default)
        {
            TClient client = await GetClientFor(dataStoreId, tenant);
            return await query(client);
        }

        async Task<TClient> GetClientFor(DataStoreId dataStoreId, Tenant tenant, CancellationToken cancellation = default)
        {
            IClientFactory<TClient> clientFactory = await _dataStoreClientRegistry.ResolveClientFactory<TClient>(dataStoreId, cancellation);
            DataStore dataStore = _dataStoreRegistry.GetDataStore(dataStoreId);
            return await clientFactory.CreateForStore(dataStore, tenant);
        }
    }
}
