using LowKey.Data.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public interface ICommandSession<out TClient>
    {
        Task Execute(DataStoreId dataStoreId, Func<TClient, Task> command, CancellationToken cancellation = default);
        Task<TResult> Execute<TResult>(DataStoreId dataStoreId, Func<TClient, Task<TResult>> command, CancellationToken cancellation = default);
    }
}
