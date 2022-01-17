using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class TenantedSession<TClient> : ITenantedCommandSession<TClient>, ITenantedQuerySession<TClient>
    {
        DataStoreClientFactoryRegistry _dataStoreClientRegistry;

        public TenantedSession(DataStoreClientFactoryRegistry dataStoreClientRegistry)
        {
            _dataStoreClientRegistry = dataStoreClientRegistry;
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
            var clientFactory = await _dataStoreClientRegistry.ResolveClientFactory<TClient>(dataStoreId, cancellation);
            return await clientFactory.CreateForStore(dataStoreId, tenant);
        }
    }
}
