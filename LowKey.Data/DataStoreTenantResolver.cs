using LowKey.Data.Model;
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
            ITenantIdResolver tenantIdResolver = await _dataStoreTanantResolverRegistry.GetTenantIdResolverFor(dataStoreId, cancellation);
            TenantId tenantId = await tenantIdResolver.Resolve();

            ITenantResolver tenantResolver = await _dataStoreTanantResolverRegistry.GetTenantResolverFor(dataStoreId, cancellation);
            Tenant tenant = await tenantResolver.Resolve(dataStoreId, tenantId, cancellation);
            return tenant;
        }
    }
}