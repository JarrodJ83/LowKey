using LowKey.Data.Model;
using System;
using System.Threading.Tasks;

namespace LowKey.Data.MultiTenancy
{
    public class AmbientContextTenantIdResolver : ITenantIdResolver
    {
        public Task<TenantId> Resolve()
        {
            var tenantId = TenantIdContext.Current?.TenantId;

            if (tenantId == null) throw new InvalidOperationException("TenantContext is not set");

            return Task.FromResult(tenantId);
        }
    }
}
