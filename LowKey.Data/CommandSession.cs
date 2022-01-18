using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    class CommandSession<TClient> : ICommandSession<TClient>
    {
        private readonly ITenantedCommandSession<TClient> _tenantedCommandSession;
        private readonly DataStoreTanantResolverRegistry _dataStoreTanantResolverRegistry;

        public CommandSession(ITenantedCommandSession<TClient> tenantedCommandSession, DataStoreTanantResolverRegistry dataStoreTanantResolverRegistry)
        {
            _tenantedCommandSession = tenantedCommandSession;
            _dataStoreTanantResolverRegistry = dataStoreTanantResolverRegistry;
        }

        public async Task Execute<TResult>(DataStoreId dataStoreId, Func<TClient, Task> command, CancellationToken cancellation = default)
        {
            Tenant tenant = await ResolveTenant(dataStoreId, cancellation);
            await _tenantedCommandSession.Execute(dataStoreId, tenant, command, cancellation);
        }

        private async Task<Tenant> ResolveTenant(DataStoreId dataStoreId, CancellationToken cancellation)
        {
            ITenantResolver tenantResolver = await _dataStoreTanantResolverRegistry.GetTenantResolverFor(dataStoreId, cancellation);
            Tenant tenant = await tenantResolver.Resolve(dataStoreId, cancellation);
            return tenant;
        }
    }
}
