using LowKey.Data;
using LowKey.Data.UnitTests;
using LowKey.Data.Extensions.Hosting;

namespace Microsoft.Extensions.Hosting
{
    public static class LowKeyDataStoreConfigExtensions
    {
        public static LowKeyDataStoreConfig WithTestClient(this LowKeyDataStoreConfig config)
        {
            config.UseClientFactory(() => (IClientFactory<TestClient>)new TestClientFactory());

            return config;
        }
    }
}
