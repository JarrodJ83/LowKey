using LowKey.Data.Model;

namespace LowKey.Data.Sql
{
    public record SqlServerTenant : Tenant
    {
        public SqlServerTenant(string server, int? port = null) : this(server, server, port) { }

        public SqlServerTenant(string id, string server, int? port = null) : this(new TenantId(id), server, port) { }

        public SqlServerTenant(TenantId Id, string Server, int? Port = 4133) : base(Id, Server, Port) { }
    }
}
