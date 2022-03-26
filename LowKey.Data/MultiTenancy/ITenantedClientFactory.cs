using LowKey.Data.Model;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data.MultiTenancy
{
    public interface ITenantedClientFactory
    {
        Task<TClient> GetClientFor<TClient>(DataStoreId dataStoreId, Tenant tenant, CancellationToken cancellation = default);
    }
}
