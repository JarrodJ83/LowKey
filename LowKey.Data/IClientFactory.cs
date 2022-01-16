using System.Threading.Tasks;

namespace LowKey.Data
{
    public interface IClientFactory<TClient>
    {
        Task<TClient> CreateForStore(Tenant tenant);
    }
}
