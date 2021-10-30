﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data.Diagnostics
{
    public class ActivityCommandSession<TClient> : ICommandSession<TClient>
    {
        private readonly ICommandSession<TClient> _commandSession;
        
        public ActivityCommandSession(ICommandSession<TClient> commandSession)
        {
            _commandSession = commandSession;
        }

        public async Task Execute(Db db, Func<TClient, Task> command, CancellationToken cancellation = default)
        {
            using var activity = ActivitySources.SessionActivity.StartActivity($"{nameof(Execute)} {typeof(TClient).FullName} Command", System.Diagnostics.ActivityKind.Client);

            if (activity?.IsAllDataRequested == true)
            {
                activity.SetTag(OpenTelemetryDatabaseTags.DatabaseServer, db.Server);
                activity.SetTag(OpenTelemetryDatabaseTags.DatabasePort, db.Port);
                activity.SetTag(OpenTelemetryDatabaseTags.DatabaseName, db.Name);
                activity.SetTag(OpenTelemetryDatabaseTags.DatabaseOperation, "Command");
            }

            // Don't return a task here or it will not be executed inside of the ActivitySource!
            await _commandSession.Execute(db, command, cancellation);
        }
    }
}
