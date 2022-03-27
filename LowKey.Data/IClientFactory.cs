using LowKey.Data.Model;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public interface IClientFactory
    {
        Task<TClient> Create<TClient>(DataStoreId dataStoreId, CancellationToken cancel = default);
    }

    public interface IClientFactory<TClient>
    {
        Task<TClient> CreateForStore(DataStore dataStore, Tenant tenant);
    }
}
