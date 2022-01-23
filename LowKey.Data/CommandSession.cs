using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class CommandSession<TClient> : ICommandSession<TClient>
    {
        private readonly ITenantIdResolver _tenantIdResolver;
        private readonly ITenantedCommandSession<TClient> _tenantedCommandSession;
        private readonly DataStoreTanantResolverRegistry _dataStoreTanantResolverRegistry;

        public CommandSession(ITenantedCommandSession<TClient> tenantedCommandSession, ITenantIdResolver tenantIdResolver, DataStoreTanantResolverRegistry dataStoreTanantResolverRegistry)
        {
            _tenantedCommandSession = tenantedCommandSession;
            _tenantIdResolver = tenantIdResolver;
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
            var tenantId = await _tenantIdResolver.Resolve();
            Tenant tenant = await tenantResolver.Resolve(dataStoreId, tenantId, cancellation);
            return tenant;
        }
    }
}
