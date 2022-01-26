namespace LowKey.Data.Model
{
    public record DataStore(DataStoreId Id, string Name)
    {
        public DataStore(string id, string name) : this(new DataStoreId(id), name) { }
        public DataStore(string nameAndId) : this(new DataStoreId(nameAndId), nameAndId)
        {
        }
    }
}
