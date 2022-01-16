namespace LowKey.Data
{
    public record DataStoreId(string Name);
    public record Tenant(string Name, string Server, int? Port = null);
}
