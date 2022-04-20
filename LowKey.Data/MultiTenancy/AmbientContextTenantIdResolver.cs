using LowKey.Data.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data.MultiTenancy
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
        private static readonly AsyncLocal<TenantContext?> _tenantContext = new AsyncLocal<TenantContext?>();
        public TenantId TenantId { get; private set; }
        public static TenantContext? Current => _tenantContext.Value;

        private TenantContext(TenantId tenantId)
        {
            TenantId = tenantId;
        }

        public static TenantContext CreateFor(TenantId tenantId) { 
            var ctx = new TenantContext(tenantId);

            _tenantContext.Value = ctx;

            return ctx;
        }
        public void Dispose()
        {
            _tenantContext.Value = null;
        }
    }
}
