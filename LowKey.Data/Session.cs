using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class Session<TClient> : IQuerySession<TClient>
    {
        IClientFactory<TClient> _clientFactory;

        public Session(IClientFactory<TClient> clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public Task<TResult> Execute<TResult>(DataStoreId dataStoreId, Func<TClient, Task<TResult>> query, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }
    }
}
