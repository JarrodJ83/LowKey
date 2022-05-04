namespace LowKey.Data.Model
{
    public record TenantId(string Value)
    {
        public static TenantId Empty = new(string.Empty);
        public static TenantId From(string value) => new(value);
    }
}
