using System;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class TenantedSessionTests
    {
        DataStoreId TestDataStore = new DataStoreId("Test");
        Tenant TestTenant = new("", "", 0);
        TenantedSession<TestClient> _session;
        IClientFactory<TestClient> _clientFactory;
        DataStoreClientFactoryRegistry _dataStoreClientFactoryRegistry;
        public TenantedSessionTests()
        {
            _dataStoreClientFactoryRegistry = new DataStoreClientFactoryRegistry();
            _clientFactory = new TestClientFactory();
            _session = new TenantedSession<TestClient>(_dataStoreClientFactoryRegistry);
        }

        [Fact]
        public async Task SessionCreatesClientAndExecutesCommand()
        {
            GivenClientFactoryFor(TestDataStore, _clientFactory);
            TestClient? testClient = null;

            await _session.Execute(TestDataStore, TestTenant, client => {
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

            var result = await _session.Execute(TestDataStore, TestTenant, client => {
                testClient = client;
                return Task.FromResult(new TestResult());
            });

            Assert.NotNull(testClient);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ClientNotRegisteredForDataStoreThrowsException()
        {
            TestClient? testClient = null;

            await Assert.ThrowsAsync<InvalidOperationException>(() => _session.Execute(TestDataStore, TestTenant, client => {
                return Task.FromResult(new TestResult());
            }));
        }

        void GivenClientFactoryFor<TClient>(DataStoreId dataStoreId, IClientFactory<TClient> clientFactory) => 
            _dataStoreClientFactoryRegistry.RegisterClientFor<TClient>(dataStoreId, cancel => Task.FromResult(clientFactory));

        record TestResult;
    }
}
