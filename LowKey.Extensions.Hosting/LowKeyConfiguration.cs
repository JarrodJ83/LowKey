using LowKey.Data;
using System.Threading.Tasks;

namespace LowKey.Extensions.Hosting
{
    public class LowKeyConfiguration
    {
        public DataStoreTanantResolverRegistry DataStoreTanantResolverRegistry { get; }

        public LowKeyConfiguration()
        {
            DataStoreTanantResolverRegistry = new DataStoreTanantResolverRegistry();
        }

        public LowKeyConfiguration AddStore(string dataStoreId, string server, int? port = null) =>
            AddStore(new DataStoreId(dataStoreId), new Tenant("default", server, port));

        public LowKeyConfiguration AddStore(DataStoreId dataStoreId, Tenant tenant)
        {
            DataStoreTanantResolverRegistry.RegisterTenantResolverFor(dataStoreId, 
                cancel => Task.FromResult((ITenantResolver)new SingleTenantResolver(tenant)));

            return this;
        }
    }
}
