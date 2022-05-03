using AutoFixture.Xunit2;
using LowKey.Data.Model;
using LowKey.Data.MultiTenancy;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class TenantedClientFactoryTests
    {
        DataStoreRegistry _dataStoreRegistry;
        DataStoreClientFactoryRegistry _dataStoreClientFactoryRegistry;
        TenantedClientFactory _clientFactory;

        public TenantedClientFactoryTests()
        {
            _dataStoreRegistry = new DataStoreRegistry();
            _dataStoreClientFactoryRegistry = new DataStoreClientFactoryRegistry();
            _clientFactory = new TenantedClientFactory(_dataStoreClientFactoryRegistry, _dataStoreRegistry);
        }

        [Theory, AutoData]
        public async Task ClientCreatesClientForCorrectDataStore(DataStore dataStore, Tenant tenant)
        {
            GivenDataStoreRegistered(dataStore);
            GivenClientFactoryRegisteredForDataStore(dataStore.Id);

            var client = await _clientFactory.GetClientFor<TestClient>(dataStore.Id, tenant);

            Assert.Equal(tenant, client.Tenant);
            Assert.Equal(dataStore, client.DataStore);
        }

        void GivenDataStoreRegistered(DataStore dataStore) => _dataStoreRegistry.Add(dataStore);

        void GivenClientFactoryRegisteredForDataStore(DataStoreId dataStore) => 
            _dataStoreClientFactoryRegistry.RegisterClientFor(dataStore, () => (IClientFactory<TestClient>)new TestClientFactory());
    }
}
