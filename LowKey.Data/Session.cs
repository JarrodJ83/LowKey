using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class Session<TClient> : ICommandSession<TClient>, IQuerySession<TClient>
    {
        IClientFactory<TClient> _clientFactory;

        public Session(IClientFactory<TClient> clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task Execute(Db db, Func<TClient, Task> command, CancellationToken cancellation = default)
        {
            TClient client = await _clientFactory.CreateForStore(db);
            await command(client);
        }

        public async Task<TResult> Execute<TResult>(Db db, Func<TClient, Task<TResult>> query, CancellationToken cancellation = default)
        {
            TClient client = await _clientFactory.CreateForStore(db);
            return await query(client);
        }
    }
}
