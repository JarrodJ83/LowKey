using LowKey.Data;
using LowKey.Data.Model;
using LowKey.Data.UnitTests;
using LowKey.Extensions.Hosting;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting
{
    public static class LowKeyConfigurationExtensions
    {
        public static LowKeyConfiguration WithTestClient(this LowKeyConfiguration config, DataStoreId dataStoreId)
        {
            config.DataStoreClientFactoryRegistry.RegisterClientFor(dataStoreId, cancel => Task.FromResult((IClientFactory<TestClient>)new TestClientFactory()));

            return config;
        }
    }
}
