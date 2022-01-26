using LowKey.Data.Model;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public interface ITenantIdResolver
    {
        Task<TenantId> Resolve();
    }
}
