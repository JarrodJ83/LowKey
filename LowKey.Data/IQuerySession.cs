using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public interface IQuerySession<TClient>
    {
        Task<TResult> Execute<TResult>(Db db, Func<TClient, Task<TResult>> query, CancellationToken cancellation = default);
    }
}
