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
        public async Task ClientCreatedForCorrectClientAndTenant(DataStore dataStore, Tenant tenant)
        {
            GivenDataStoreIsRegistered(dataStore);
            GivenSingleTenenatRegisteredForDataStore(dataStore.Id, tenant);

            TestClient client = await WhenClientIsCreatedFor(dataStore.Id);

            Assert.Equal(client.Tenant, tenant);
            Assert.Equal(client.DataStore, dataStore);
        }

        private Task<TestClient> WhenClientIsCreatedFor(DataStoreId dataStoreId) =>
            _clientFactory.Create<TestClient>(dataStoreId);

        private void GivenSingleTenenatRegisteredForDataStore(DataStoreId id, Tenant tenant)
        {
            TenantContext.CreateFor(tenant.Id);

            var registeredTenants = new Dictionary<DataStoreId, Tenant[]>
            {
                { id, new[] { tenant }  }
            };

            ITenantResolver tenantResolver = new InMemoryTenantResolver(registeredTenants);
            ITenantIdResolver tanantIdResolver = new AmbientContextTenantIdResolver();

            _dataStoreTenantResolverRegistry.RegisterTenantResolverFor(id,
                cancel => Task.FromResult(tenantResolver),
                cancel => Task.FromResult(tanantIdResolver));
        }

        private void GivenDataStoreIsRegistered(DataStore dataStore)
        {
            _clientFactoryRegistry.RegisterClientFor(dataStore.Id,
                   cancel => Task.FromResult((IClientFactory<TestClient>)new TestClientFactory()));

            _dataStoreRegistry.Add(dataStore);
        }
    }
}
