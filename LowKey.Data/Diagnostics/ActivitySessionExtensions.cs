using System;
using System.Diagnostics;

namespace LowKey.Data.Diagnostics
{
    static class ActivitySessionExtensions
    {
        public static void SetLowKeyActivityTags(this Activity? activity, Db db, Type clientType, string operation)
        {
            if (activity?.IsAllDataRequested == true)
            {
                activity.SetTag(OpenTelemetryDatabaseTags.DatabaseServer, db.Server);
                activity.SetTag(OpenTelemetryDatabaseTags.DatabasePort, db.Port);
                activity.SetTag(OpenTelemetryDatabaseTags.DatabaseName, db.Name);
                activity.SetTag(OpenTelemetryDatabaseTags.DatabaseOperation, operation);
                activity.SetTag(LowKeyDataActivityTags.ClientType, clientType.FullName);
            }
        }
    }
}
