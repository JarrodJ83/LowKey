using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class SessionTests
    {
        Db TestDb = new("", "", 0);
        Session<TestClient> _session;
        IClientFactory<TestClient> _clientFactory;

        public SessionTests()
        {
            _clientFactory = new TestClientFactory();
            _session = new Session<TestClient>(_clientFactory);
        }

        [Fact]
        public async Task SessionCreatesClientAndExecutesCommand()
        {
            TestClient testClient = null;

            await _session.Execute(TestDb, client => {
                testClient = client;
                return Task.CompletedTask;
            });

            Assert.NotNull(testClient);
        }

        [Fact]
        public async Task SessionCreatesClientAndExecutesQueryWithResult()
        {
            TestClient testClient = null;

            var result = await _session.Execute(TestDb, client => {
                testClient = client;
                return Task.FromResult(new TestResult());
            });

            Assert.NotNull(testClient);
            Assert.NotNull(result);
        }
        
        record TestResult;
    }
}
