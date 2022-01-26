using LowKey.Data.Model;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data.MultiTenancy
{
    public interface IDataStoreTenantResolver
    {
        Task<Tenant> Resolve(DataStoreId dataStoreId, CancellationToken cancel = default);
    }
}