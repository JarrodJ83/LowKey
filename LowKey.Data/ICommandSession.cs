using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public interface ICommandSession<TClient>
    {
        Task Execute<TResult>(DataStoreId dataStoreId, Func<TClient, Task> command, CancellationToken cancellation = default);
    }
}
