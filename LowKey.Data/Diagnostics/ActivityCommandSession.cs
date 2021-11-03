using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data.Diagnostics
{
    public class ActivityCommandSession<TClient> : ICommandSession<TClient>
    {
        private readonly ICommandSession<TClient> _commandSession;
        private const string Operation = "Command";

        public ActivityCommandSession(ICommandSession<TClient> commandSession)
        {
            _commandSession = commandSession;
        }

        public Task Execute(Db db, Func<TClient, Task> command, CancellationToken cancellation = default)
        {
            using Activity? activity = ActivitySources.SessionActivity.StartActivity($"{nameof(Execute)} {typeof(TClient).FullName} {Operation}", ActivityKind.Client);

            activity.SetLowKeyActivityTags(db, typeof(TClient), Operation);

            return _commandSession.Execute(db, command, cancellation);
        }
    }
}
