using LowKey.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class TenantedSessionTests
    {
        DataStore TestDataStore = new("Test");
        Tenant TestTenant = new("", "", 0);
        TenantedSession<TestClient> _session;
        IClientFactory<TestClient> _clientFactory;
        DataStoreClientFactoryRegistry _dataStoreClientFactoryRegistry;
        public TenantedSessionTests()
        {
            _dataStoreClientFactoryRegistry = new DataStoreClientFactoryRegistry();
            _clientFactory = new TestClientFactory();
            _session = new TenantedSession<TestClient>(_dataStoreClientFactoryRegistry, 
                new DataStoreRegistry(new HashSet<DataStore> { TestDataStore }));
        }

        [Fact]
        public async Task SessionCreatesClientAndExecutesCommand()
        {
            GivenClientFactoryFor(TestDataStore, _clientFactory);
            TestClient? testClient = null;

            await _session.Execute(TestDataStore.Id, TestTenant, client => {
                testClient = client;
                return Task.CompletedTask;
            });

            Assert.NotNull(testClient);
        }

        [Fact]
        public async Task SessionCreatesClientAndExecutesQueryWithResult()
        {
            GivenClientFactoryFor(TestDataStore, _clientFactory);
            TestClient? testClient = null;

            var result = await _session.Execute(TestDataStore.Id, TestTenant, client => {
                testClient = client;
                return Task.FromResult(new TestResult());
            });

            Assert.NotNull(testClient);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ClientNotRegisteredForDataStoreThrowsException()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => _session.Execute(TestDataStore.Id, TestTenant, client => {
                return Task.FromResult(new TestResult());
            }));
        }

        void GivenClientFactoryFor<TClient>(DataStore dataStore, IClientFactory<TClient> clientFactory) => 
            _dataStoreClientFactoryRegistry.RegisterClientFor<TClient>(dataStore.Id, cancel => Task.FromResult(clientFactory));

        record TestResult;
    }
}
