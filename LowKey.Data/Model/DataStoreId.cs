namespace LowKey.Data.Model
{
    public record DataStoreId(string Value)
    {
        public static DataStoreId Empty = new DataStoreId(string.Empty);
    }
}
