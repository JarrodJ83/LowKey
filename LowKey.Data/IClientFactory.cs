using LowKey.Data.Model;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public interface IClientFactory<TClient>
    {
        Task<TClient> CreateForStore(DataStore dataStore, Tenant tenant);
    }
}
