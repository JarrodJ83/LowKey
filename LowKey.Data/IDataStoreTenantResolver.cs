using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public interface IDataStoreTenantResolver
    {
        Task<Tenant> Resolve(DataStoreId dataStoreId, CancellationToken cancel = default);
    }
}