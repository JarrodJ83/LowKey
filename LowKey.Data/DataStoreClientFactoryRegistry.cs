using LowKey.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class DataStoreClientFactoryRegistry
    {
        private readonly Dictionary<DataStoreId, object> _dataStoreClientTypes = new();

        public void RegisterClientFor<TClient>(DataStoreId dataStore, Func<CancellationToken, Task<IClientFactory<TClient>>> clientFactoryResolver)
        {
            _dataStoreClientTypes.Add(dataStore, clientFactoryResolver);
        }

        public Task<IClientFactory<TClient>> ResolveClientFactory<TClient>(DataStoreId dataStore, CancellationToken cancellation = default)
        {
            if(_dataStoreClientTypes.TryGetValue(dataStore, out var clientFactoryResolver))
            {
                var resolver = (Func<CancellationToken, Task<IClientFactory<TClient>>>)clientFactoryResolver;
                return resolver(cancellation);
            }

            throw new InvalidOperationException($"No {nameof(IClientFactory<TClient>)} registered for \"{dataStore.Value}\" DataStore");
        }
    }
}
