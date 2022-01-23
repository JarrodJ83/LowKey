using System.Threading.Tasks;

namespace LowKey.Data
{
    public interface ITenantIdResolver
    {
        Task<TenantId> Resolve();
    }
}
