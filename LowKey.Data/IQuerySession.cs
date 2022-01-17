using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public interface IQuerySession<TClient>
    {
        Task<TResult> Execute<TResult>(DataStoreId dataStoreId, Func<TClient, Task<TResult>> query, CancellationToken cancellation = default);
    }
}
