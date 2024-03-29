﻿using LowKey.Data.Model;
using System;
using System.Collections.Generic;

namespace LowKey.Data
{
    public class DataStoreClientFactoryRegistry
    {
        private readonly Dictionary<(DataStoreId, Type), object> _dataStoreClientTypes = new();

        public void RegisterClientFor<TClient>(DataStoreId dataStore, Func<IClientFactory<TClient>> clientFactoryResolver)
        {
            _dataStoreClientTypes.Add((dataStore, typeof(TClient)), clientFactoryResolver);
        }

        public IClientFactory<TClient> ResolveClientFactory<TClient>(DataStoreId dataStore)
        {
            if(_dataStoreClientTypes.TryGetValue((dataStore, typeof(TClient)), out var clientFactoryResolver))
            {
                var resolver = (Func<IClientFactory<TClient>>)clientFactoryResolver;
                return resolver();
            }

            throw new InvalidOperationException($"No {nameof(IClientFactory<TClient>)} registered for \"{dataStore.Value}\" DataStore");
        }
    }
}
