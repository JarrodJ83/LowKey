using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public interface ITenantedQuerySession<TClient>
    {
        Task<TResult> Execute<TResult>(DataStoreId dataStoreId, Tenant tenant, Func<TClient, Task<TResult>> query, CancellationToken cancellation = default);
    }
}
