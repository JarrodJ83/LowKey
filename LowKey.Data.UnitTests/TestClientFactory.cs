using System.Threading.Tasks;

namespace LowKey.Data.UnitTests
{
    class TestClientFactory : IClientFactory<TestClient>
    {
        public Task<TestClient> CreateForStore(Db db)
        {
            return Task.FromResult(new TestClient(db));
        }
    }
}
