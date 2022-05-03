using LowKey.Data;
using LowKey.Data.Model;
using System;

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

        public void UseClientFactory<TClient>(Func<IClientFactory<TClient>> clientFactoryResolver)
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

        public LowKeyDataStoreConfig AddStore(string dataStore, Func<ITenantResolver> tenantResolverFactory, Func<ITenantIdResolver> tenantIdResolverFactory) =>
            AddStore(new DataStore(dataStore), tenantResolverFactory, tenantIdResolverFactory);

        public LowKeyDataStoreConfig AddStore(DataStore dataStore, Func<ITenantResolver> tenantResolverFactory, Func<ITenantIdResolver> tenantIdResolverFactory)
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
                () => new SingleTenantResolver(tenant),
                () => new SingleTenantIdResolver(tenant.Id));

            return new LowKeyDataStoreConfig(this, dataStore);
        }
    }
}
