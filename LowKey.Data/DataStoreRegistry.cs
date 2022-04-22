using LowKey.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LowKey.Data
{
    public class DataStoreRegistry
    {
        private readonly ISet<DataStore> _dataStores;

        public DataStoreRegistry(ISet<DataStore>? dataStores = null)
        {
            _dataStores = dataStores ?? new HashSet<DataStore>();
        }

        public void Add(DataStore dataStore) => _dataStores.Add(dataStore);

        public DataStore GetDataStore(DataStoreId dataStoreId)
        {
            var dataStore = _dataStores.SingleOrDefault(dataStore => dataStore.Id == dataStoreId);

            if (dataStore == null) throw new InvalidOperationException($"DataStore {dataStoreId.Value} not registered");

            return dataStore;
        }
    }
}
