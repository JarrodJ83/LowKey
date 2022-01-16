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

        public TenantedSessionTests()
        {
            _clientFactory = new TestClientFactory();
            _session = new TenantedSession<TestClient>(_clientFactory);
        }

        [Fact]
        public async Task SessionCreatesClientAndExecutesCommand()
        {
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
            TestClient? testClient = null;

            var result = await _session.Execute(TestDataStore, TestTenant, client => {
                testClient = client;
                return Task.FromResult(new TestResult());
            });

            Assert.NotNull(testClient);
            Assert.NotNull(result);
        }

        record TestResult;
    }
}
