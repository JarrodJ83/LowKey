using LowKey.Data.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class InMemoryTenantResolver : ITenantResolver
    {
        private readonly IReadOnlyDictionary<DataStoreId, Tenant[]> _dataStoreTenants;
        public InMemoryTenantResolver(IDictionary<DataStoreId, Tenant[]> dataStoreTenants)
        {
            _dataStoreTenants = new ReadOnlyDictionary<DataStoreId, Tenant[]>(dataStoreTenants);
        }

        public Task<Tenant> Resolve(DataStoreId dataStoreId, TenantId tenantId, CancellationToken cancel = default)
        {
            if(_dataStoreTenants.TryGetValue(dataStoreId, out var dataStoreTenants))
            {
                Tenant? tenant = dataStoreTenants.SingleOrDefault(t => t.Id == tenantId);

                if(tenant == null)
                {
                    throw new InvalidOperationException($"Tenant {tenantId.Value} not found");
                }

                return Task.FromResult(tenant);
            }

            throw new InvalidOperationException($"DataStore {tenantId.Value} not found");
        }
    }
}
