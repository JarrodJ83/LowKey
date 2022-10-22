using LowKey.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LowKey.Data
{
    public class DataStoreRegistry
    {
        private readonly IDictionary<DataStoreId, DataStore> _dataStores;

        public DataStoreRegistry(ISet<DataStore>? dataStores = null)
        {
            dataStores ??= new HashSet<DataStore>();

            _dataStores =  dataStores.ToDictionary(ds => ds.Id, ds => ds);
        }

        public void Add(DataStore dataStore) => _dataStores.Add(dataStore.Id, dataStore);

        public DataStore GetDataStore(DataStoreId dataStoreId)
        {
            if(_dataStores.TryGetValue(dataStoreId, out var dataStore))
            {
                return dataStore;
            }

            throw new DataStoreNotRegisteredException(dataStoreId);
        }
    }
}
