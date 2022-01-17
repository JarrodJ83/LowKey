using System.Threading.Tasks;

namespace LowKey.Data.UnitTests
{
    class TestClientFactory : IClientFactory<TestClient>
    {
        public Task<TestClient> CreateForStore(DataStoreId dataStoreId, Tenant tenant)
        {
            return Task.FromResult(new TestClient(dataStoreId, tenant));
        }
    }
}
