using LowKey.Data.Model;
using LowKey.Data.MultiTenancy;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class DataStoreTenantResolver : IDataStoreTenantResolver
    {
        private readonly DataStoreTanantResolverRegistry _dataStoreTanantResolverRegistry;
        public DataStoreTenantResolver(DataStoreTanantResolverRegistry dataStoreTanantResolverRegistry)
        {
            _dataStoreTanantResolverRegistry = dataStoreTanantResolverRegistry;
        }

        public async Task<Tenant> Resolve(DataStoreId dataStoreId, CancellationToken cancellation = default)
        {
            ITenantIdResolver tenantIdResolver = _dataStoreTanantResolverRegistry.GetTenantIdResolverFor(dataStoreId);
            TenantId tenantId = await tenantIdResolver.Resolve();

            ITenantResolver tenantResolver = _dataStoreTanantResolverRegistry.GetTenantResolverFor(dataStoreId);
            Tenant tenant = await tenantResolver.Resolve(dataStoreId, tenantId, cancellation);
            return tenant;
        }
    }
}