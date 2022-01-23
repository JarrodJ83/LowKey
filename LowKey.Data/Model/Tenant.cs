namespace LowKey.Data.Model
{
    public record Tenant(TenantId Id, string Server, int? Port = null)
    {
        public Tenant(string id, string server, int? port = null) :
            this(new TenantId(id), server, port)
        {

        }
    }
}
