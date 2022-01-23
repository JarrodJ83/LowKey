using LowKey.Data;
using LowKey.Data.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Extensions.Hosting
{
    public class LowKeyDataStoreConfig
    {
        public DataStoreId DataStoreId { get; }

        private LowKeyConfiguration _parentConfiguration;

        public LowKeyDataStoreConfig(LowKeyConfiguration parentConfig, DataStoreId dataStoreId)
        {
            _parentConfiguration = parentConfig;
            DataStoreId = dataStoreId;
        }

        public void UseClientFactory<TClient>(Func<CancellationToken, Task<IClientFactory<TClient>>> clientFactoryResolver)
        {
            _parentConfiguration.DataStoreClientFactoryRegistry.RegisterClientFor(DataStoreId, clientFactoryResolver);
        }
    }
    public class LowKeyConfiguration
    {
        public DataStoreTanantResolverRegistry DataStoreTanantResolverRegistry { get; }
        public DataStoreClientFactoryRegistry DataStoreClientFactoryRegistry { get; }


        public LowKeyConfiguration()
        {
            DataStoreTanantResolverRegistry = new();
            DataStoreClientFactoryRegistry = new();
        }

        public LowKeyDataStoreConfig AddStore(string dataStoreId, Func<CancellationToken, Task<ITenantResolver>> tenantResolverFactory, Func<CancellationToken, Task<ITenantIdResolver>> tenantIdResolverFactory) =>
            AddStore(new DataStoreId(dataStoreId), tenantResolverFactory, tenantIdResolverFactory);

        public LowKeyDataStoreConfig AddStore(DataStoreId dataStoreId, Func<CancellationToken, Task<ITenantResolver>> tenantResolverFactory, Func<CancellationToken, Task<ITenantIdResolver>> tenantIdResolverFactory)
        {
            DataStoreTanantResolverRegistry.RegisterTenantResolverFor(dataStoreId, tenantResolverFactory, tenantIdResolverFactory);

            return new LowKeyDataStoreConfig(this, dataStoreId);
        }

        public LowKeyDataStoreConfig AddStore(string dataStoreId, string server, string tenant, int? port = null) =>
            AddStore(new DataStoreId(dataStoreId), new Tenant(tenant, server, port));

        public LowKeyDataStoreConfig AddStore(DataStoreId dataStoreId, string server, string tenant, int? port = null) =>
        AddStore(dataStoreId, new Tenant(tenant, server, port));


        public LowKeyDataStoreConfig AddStore(DataStoreId dataStoreId, Tenant tenant)
        {
            DataStoreTanantResolverRegistry.RegisterTenantResolverFor(dataStoreId,
                cancel => Task.FromResult((ITenantResolver)new SingleTenantResolver(tenant)),
                cancel => Task.FromResult((ITenantIdResolver)new SingleTenantIdResolver(tenant.Id)));

            return new LowKeyDataStoreConfig(this, dataStoreId);
        }
    }
}
