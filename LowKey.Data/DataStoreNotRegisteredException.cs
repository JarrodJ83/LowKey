using LowKey.Data.Model;

namespace LowKey.Data
{
    internal class DataStoreNotRegisteredException : Exception
    {
        public DataStoreId DataStoreId { get; private set; }
        public DataStoreNotRegisteredException(DataStoreId dataStoreId) :
            base($"DataStore {dataStoreId} is not registered. Please check your configuration")
        {
            DataStoreId = dataStoreId;
        }
    }
}
