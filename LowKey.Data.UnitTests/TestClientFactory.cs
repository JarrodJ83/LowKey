using LowKey.Data.Model;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data.UnitTests
{
    class TestClientFactory : IClientFactory<TestClient>
    {
        public Task<TestClient> CreateForStore(DataStore dataStore, Tenant tenant, CancellationToken cancel)
        {
            return Task.FromResult(new TestClient(dataStore, tenant));
        }
    }
}
