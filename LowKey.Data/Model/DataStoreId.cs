namespace LowKey.Data.Model
{
    public record DataStoreId(string Value);
    public record DataStore(DataStoreId Id, string Name)
    {
        public DataStore(string nameAndId) : this(new DataStoreId(nameAndId), nameAndId)
        {
        }
    }
}
