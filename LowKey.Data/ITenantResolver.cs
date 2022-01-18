using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public interface ITenantResolver
    {
        Task<Tenant> Resolve(DataStoreId dataStoreId, CancellationToken cancel = default);
    }
}