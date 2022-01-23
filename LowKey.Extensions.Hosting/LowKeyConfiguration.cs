using LowKey.Data;
using LowKey.Data.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Extensions.Hosting
{
    public class LowKeyConfiguration
    {
        public DataStoreTanantResolverRegistry DataStoreTanantResolverRegistry { get; }
        public DataStoreClientFactoryRegistry DataStoreClientFactoryRegistry { get; }

        public LowKeyConfiguration()
        {
            DataStoreTanantResolverRegistry = new();
            DataStoreClientFactoryRegistry = new();
        }

        public LowKeyConfiguration AddStore(string dataStoreId, Func<CancellationToken, Task<ITenantResolver>> tenantResolverFactory, Func<CancellationToken, Task<ITenantIdResolver>> tenantIdResolverFactory) =>
            AddStore(new DataStoreId(dataStoreId), tenantResolverFactory, tenantIdResolverFactory);

        public LowKeyConfiguration AddStore(DataStoreId dataStoreId, Func<CancellationToken, Task<ITenantResolver>> tenantResolverFactory, Func<CancellationToken, Task<ITenantIdResolver>> tenantIdResolverFactory)
        {
            DataStoreTanantResolverRegistry.RegisterTenantResolverFor(dataStoreId, tenantResolverFactory, tenantIdResolverFactory);

            return this;
        }

        public LowKeyConfiguration AddStore(string dataStoreId, string server, string tenant, int? port = null) =>
            AddStore(new DataStoreId(dataStoreId), new Tenant(tenant, server, port));

        public LowKeyConfiguration AddStore(DataStoreId dataStoreId, string server, string tenant, int? port = null) =>
        AddStore(dataStoreId, new Tenant(tenant, server, port));


        public LowKeyConfiguration AddStore(DataStoreId dataStoreId, Tenant tenant)
        {
            DataStoreTanantResolverRegistry.RegisterTenantResolverFor(dataStoreId,
                cancel => Task.FromResult((ITenantResolver)new SingleTenantResolver(tenant)),
                cancel => Task.FromResult((ITenantIdResolver)new SingleTenantIdResolver(tenant.Id)));

            return this;
        }
    }
}
