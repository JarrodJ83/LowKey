using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class SingleTenantResolver : ITenantResolver
    {
        private readonly Tenant _tenant;

        public SingleTenantResolver(Tenant tenant)
        {
            _tenant = tenant;
        }

        public Task<Tenant> Resolve(DataStoreId dataStoreId, TenantId tenantId, CancellationToken cancel = default) => Task.FromResult(_tenant);
    }
}
