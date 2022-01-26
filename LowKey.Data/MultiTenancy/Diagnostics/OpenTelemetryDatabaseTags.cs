namespace LowKey.Data.MultiTenancy.Diagnostics
{
    public static class OpenTelemetryDatabaseTags
    {
        public const string DatabasePort = "net.peer.port";
        public const string DatabaseName = "db.name";
        public const string DatabaseServer = "net.peer.name";
        public const string DatabaseOperation = "db.operation";
    }
}
