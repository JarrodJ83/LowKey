using LowKey.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Extensions.Hosting
{
    public class LowKeyConfiguration
    {
        private Tenant tenant;

        public DataStoreTanantResolverRegistry DataStoreTanantResolverRegistry { get; }

        public LowKeyConfiguration()
        {
            DataStoreTanantResolverRegistry = new DataStoreTanantResolverRegistry();
        }

        public LowKeyConfiguration AddStore(string dataStoreId, Func<CancellationToken, Task<ITenantResolver>> tenantResolverFactory, int? port = null)
        {
            DataStoreTanantResolverRegistry.RegisterTenantResolverFor(new DataStoreId(dataStoreId), tenantResolverFactory);

            return this;
        }
        public LowKeyConfiguration AddStore(string dataStoreId, string server, string tenant, int? port = null) =>
            AddStore(new DataStoreId(dataStoreId), new Tenant(tenant, server, port));


        public LowKeyConfiguration AddStore(DataStoreId dataStoreId, Tenant tenant)
        {
            DataStoreTanantResolverRegistry.RegisterTenantResolverFor(dataStoreId, 
                cancel => Task.FromResult((ITenantResolver)new SingleTenantResolver(tenant)));

            return this;
        }
    }
}
