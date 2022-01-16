using System.Threading.Tasks;

namespace LowKey.Data.UnitTests
{
    class TestClientFactory : IClientFactory<TestClient>
    {
        public Task<TestClient> CreateForStore(Tenant tenant)
        {
            return Task.FromResult(new TestClient(tenant));
        }
    }
}
