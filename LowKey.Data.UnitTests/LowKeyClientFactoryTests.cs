using AutoFixture.Xunit2;
using LowKey.Data.Model;
using LowKey.Data.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class LowKeyClientFactoryTests
    {
        LowKeyClientFactory _clientFactory;
        DataStoreTenantResolver _dataStoreTenantResolver;
        DataStoreTanantResolverRegistry _dataStoreTenantResolverRegistry;
        DataStoreClientFactoryRegistry _clientFactoryRegistry;
        DataStoreRegistry _dataStoreRegistry;

        public LowKeyClientFactoryTests()
        {
            _dataStoreTenantResolverRegistry = new DataStoreTanantResolverRegistry();
            _dataStoreTenantResolver = new DataStoreTenantResolver(_dataStoreTenantResolverRegistry);
            _dataStoreRegistry = new DataStoreRegistry();
            _clientFactoryRegistry = new DataStoreClientFactoryRegistry();

            _clientFactory = new LowKeyClientFactory(_dataStoreTenantResolver, _clientFactoryRegistry, _dataStoreRegistry);
        }

        [Theory, AutoData]
        public async Task ClientCreatedForCorrectClientAndSingleTenant(DataStore dataStore, Tenant tenant)
        {
            GivenDataStoreIsRegistered(dataStore);
            GivenSingleTenenatRegisteredForDataStore(dataStore.Id, tenant);

            using (TenantIdContext.CreateFor(tenant.Id))
            {
                TestClient client = await WhenClientIsCreatedFor(dataStore.Id);

                ThenClientIsForCorrectDataStoreAndTenant(client, dataStore, tenant);
            } 
        }

        [Theory, AutoData]
        public async Task ClientCreatedForCorrectClientAndMultipleTenants(DataStore dataStore, Tenant[] tenants)
        {
            GivenDataStoreIsRegistered(dataStore);
            GivenMultipleTenenatsRegisteredForDataStore(dataStore.Id, tenants);

            foreach (var tenant in tenants)
            {
                using var tenantContext = TenantIdContext.CreateFor(tenant.Id);
                
                TestClient client = await WhenClientIsCreatedFor(dataStore.Id);

                ThenClientIsForCorrectDataStoreAndTenant(client, dataStore, tenant);
            }
        }

        private Task<TestClient> WhenClientIsCreatedFor(DataStoreId dataStoreId) =>
            _clientFactory.Create<TestClient>(dataStoreId);

        private void GivenSingleTenenatRegisteredForDataStore(DataStoreId dataStoreId, Tenant tenant) =>
            GivenMultipleTenenatsRegisteredForDataStore(dataStoreId, new Tenant[] { tenant });

        private void GivenMultipleTenenatsRegisteredForDataStore(DataStoreId dataStoreId, Tenant[] tenants)
        {            
            var registeredTenants = new Dictionary<DataStoreId, Tenant[]>
            {
                { dataStoreId, tenants }
            };

            ITenantResolver tenantResolver = new InMemoryTenantResolver(registeredTenants);
            ITenantIdResolver tanantIdResolver = new AmbientContextTenantIdResolver();

            _dataStoreTenantResolverRegistry.RegisterTenantResolverFor(dataStoreId,
                cancel => Task.FromResult(tenantResolver),
                cancel => Task.FromResult(tanantIdResolver));
        }

        private void GivenDataStoreIsRegistered(DataStore dataStore)
        {
            IClientFactory<TestClient> testClientFactory = new TestClientFactory();
            _clientFactoryRegistry.RegisterClientFor(dataStore.Id,
                   cancel => Task.FromResult(testClientFactory));

            _dataStoreRegistry.Add(dataStore);
        }

        private void ThenClientIsForCorrectDataStoreAndTenant(TestClient client, DataStore dataStore, Tenant tenant)
        {
            Assert.Equal(client.Tenant, tenant);
            Assert.Equal(client.DataStore, dataStore);
        }
    }
}
