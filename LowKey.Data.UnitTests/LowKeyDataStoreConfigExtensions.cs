using LowKey.Data;
using LowKey.Data.UnitTests;
using LowKey.Extensions.Hosting;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting
{
    public static class LowKeyDataStoreConfigExtensions
    {
        public static LowKeyDataStoreConfig WithTestClient(this LowKeyDataStoreConfig config)
        {
            config.UseClientFactory(cancel => Task.FromResult((IClientFactory<TestClient>)new TestClientFactory()));

            return config;
        }
    }
}
