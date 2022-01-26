using LowKey.Data;
using LowKey.Data.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Extensions.Hosting
{
    public class LowKeyDataStoreConfig
    {
        public DataStore DataStore { get; }

        private LowKeyConfiguration _parentConfiguration;

        public LowKeyDataStoreConfig(LowKeyConfiguration parentConfig, DataStore dataStore)
        {
            _parentConfiguration = parentConfig;
            DataStore = dataStore;
        }

        public void UseClientFactory<TClient>(Func<CancellationToken, Task<IClientFactory<TClient>>> clientFactoryResolver)
        {
            _parentConfiguration.DataStoreClientFactoryRegistry.RegisterClientFor(DataStore.Id, clientFactoryResolver);
        }
    }
    public class LowKeyConfiguration
    {
        public DataStoreTanantResolverRegistry DataStoreTanantResolverRegistry { get; }
        public DataStoreClientFactoryRegistry DataStoreClientFactoryRegistry { get; }
        public DataStoreRegistry DataStoreRegistry { get; }

        public LowKeyConfiguration()
        {
            DataStoreTanantResolverRegistry = new();
            DataStoreClientFactoryRegistry = new();
            DataStoreRegistry = new();
        }

        public LowKeyDataStoreConfig AddStore(string dataStore, Func<CancellationToken, Task<ITenantResolver>> tenantResolverFactory, Func<CancellationToken, Task<ITenantIdResolver>> tenantIdResolverFactory) =>
            AddStore(new DataStore(dataStore), tenantResolverFactory, tenantIdResolverFactory);

        public LowKeyDataStoreConfig AddStore(DataStore dataStore, Func<CancellationToken, Task<ITenantResolver>> tenantResolverFactory, Func<CancellationToken, Task<ITenantIdResolver>> tenantIdResolverFactory)
        {
            DataStoreTanantResolverRegistry.RegisterTenantResolverFor(dataStore.Id, tenantResolverFactory, tenantIdResolverFactory);
            DataStoreRegistry.Add(dataStore);

            return new LowKeyDataStoreConfig(this, dataStore);
        }

        public LowKeyDataStoreConfig AddStore(string dataStore, string server, int? port = null) =>
            AddStore(new DataStore(dataStore), new Tenant(server, server, port));

        public LowKeyDataStoreConfig AddStore(DataStore dataStore, string server, int? port = null) =>
        AddStore(dataStore, new Tenant(server, server, port));

        public LowKeyDataStoreConfig AddStore(DataStore dataStore, Tenant tenant)
        {
            DataStoreRegistry.Add(dataStore);
            DataStoreTanantResolverRegistry.RegisterTenantResolverFor(dataStore.Id,
                cancel => Task.FromResult((ITenantResolver)new SingleTenantResolver(tenant)),
                cancel => Task.FromResult((ITenantIdResolver)new SingleTenantIdResolver(tenant.Id)));

            return new LowKeyDataStoreConfig(this, dataStore);
        }
    }
}
