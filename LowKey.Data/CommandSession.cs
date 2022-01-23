using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class CommandSession<TClient> : ICommandSession<TClient>
    {
        private readonly IDataStoreTenantResolver _dataStoreTenantResolver;
        private readonly ITenantedCommandSession<TClient> _tenantedCommandSession;

        public CommandSession(ITenantedCommandSession<TClient> tenantedCommandSession, IDataStoreTenantResolver dataStoreTenantResolver)
        {
            _tenantedCommandSession = tenantedCommandSession;
            _dataStoreTenantResolver = dataStoreTenantResolver;
        }

        public async Task Execute<TResult>(DataStoreId dataStoreId, Func<TClient, Task> command, CancellationToken cancellation = default)
        {
            Tenant tenant = await _dataStoreTenantResolver.Resolve(dataStoreId, cancellation);
            await _tenantedCommandSession.Execute(dataStoreId, tenant, command, cancellation);
        }
    }
}
