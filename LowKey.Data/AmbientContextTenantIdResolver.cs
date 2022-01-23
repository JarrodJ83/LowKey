using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public class AmbientContextTenantIdResolver : ITenantIdResolver
    {
        public Task<TenantId> Resolve()
        {
            var tenantId = TenantContext.Current?.TenantId;

            if (tenantId == null) throw new InvalidOperationException("TenantContext is not set");

            return Task.FromResult(tenantId);
        }
    }

    public class TenantContext : IDisposable
    {
        private readonly AsyncLocal<TenantId?> _tenantId;
        public TenantId? TenantId => _tenantId.Value;

        public static TenantContext? Current { get; private set; }   

        private TenantContext(TenantId tenantId)
        {
            _tenantId = new();
            _tenantId.Value = tenantId;
            Current = this;
        }

        public static TenantContext CreateFor(TenantId tenantId) =>
            new TenantContext(tenantId);

        public void Dispose() 
        {   
            _tenantId.Value = null;
            Current = null;
        }
    }
}
