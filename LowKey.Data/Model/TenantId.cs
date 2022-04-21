namespace LowKey.Data.Model
{
    public record TenantId(string Value)
    {
        public static TenantId Empty = new TenantId(string.Empty);
    }
}
