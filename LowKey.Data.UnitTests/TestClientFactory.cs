using LowKey.Data.Model;
using System.Threading.Tasks;

namespace LowKey.Data.UnitTests
{
    class TestClientFactory : IClientFactory<TestClient>
    {
        public Task<TestClient> CreateForStore(DataStore dataStore, Tenant tenant)
        {
            return Task.FromResult(new TestClient(dataStore, tenant));
        }
    }
}
