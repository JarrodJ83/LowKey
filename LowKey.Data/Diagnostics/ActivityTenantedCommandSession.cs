using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data.Diagnostics
{
    public class ActivityTenantedCommandSession<TClient> : ITenantedCommandSession<TClient>
    {
        private readonly ITenantedCommandSession<TClient> _commandSession;
        private const string Operation = "Command";

        public ActivityTenantedCommandSession(ITenantedCommandSession<TClient> commandSession)
        {
            _commandSession = commandSession;
        }

        public Task Execute(DataStoreId dataStoreId, Tenant tenant, Func<TClient, Task> command, CancellationToken cancellation = default)
        {
            using Activity? activity = ActivitySources.CommandSessionActivity.StartActivity($"{nameof(Execute)} {typeof(TClient).FullName} {Operation}", ActivityKind.Client);

            activity.SetLowKeyActivityTags(dataStoreId, tenant, typeof(TClient), Operation);

            return _commandSession.Execute(dataStoreId, tenant, command, cancellation);
        }
    }
}
