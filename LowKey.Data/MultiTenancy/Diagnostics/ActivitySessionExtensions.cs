using LowKey.Data.Model;
using System;
using System.Diagnostics;

namespace LowKey.Data.MultiTenancy.Diagnostics
{
    static class ActivitySessionExtensions
    {
        public static void SetLowKeyActivityTags(this Activity? activity, DataStoreId dataStoreId, Tenant tenant, Type clientType, string operation)
        {
            if (activity?.IsAllDataRequested == true)
            {
                activity.SetTag(OpenTelemetryDatabaseTags.DatabaseServer, tenant.Server);
                activity.SetTag(OpenTelemetryDatabaseTags.DatabasePort, tenant.Port);
                activity.SetTag(OpenTelemetryDatabaseTags.DatabaseName, tenant.Id.Value);
                activity.SetTag(OpenTelemetryDatabaseTags.DatabaseOperation, operation);
                activity.SetTag(LowKeyDataActivityTags.ClientType, clientType.FullName);
            }
        }
    }
}
