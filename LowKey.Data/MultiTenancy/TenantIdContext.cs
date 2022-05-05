using LowKey.Data.Model;
using System;
using System.Collections.Immutable;
using System.Threading;

namespace LowKey.Data.MultiTenancy
{
    public class TenantIdContext : IDisposable
    {
        private static AsyncLocal<ImmutableStack<TenantIdContext>> _tenantContextStack = new(); 

        private static ImmutableStack<TenantIdContext> TenantContextStack
        {
            get
            {
                if (_tenantContextStack.Value == null)
                {
                    _tenantContextStack.Value = ImmutableStack<TenantIdContext>.Empty;
                }

                return _tenantContextStack.Value;
            }
            set
            {
                _tenantContextStack.Value = value;
            }
        }

        public static TenantIdContext? Current => 
            TenantContextStack.IsEmpty ? null : TenantContextStack.Peek();

        public TenantId TenantId { get; private set; }
       
        private TenantIdContext(TenantId tenantId)
        {
            TenantId = tenantId;
        }

        public static IDisposable CreateFor(TenantId tenantId) { 
            var ctx = new TenantIdContext(tenantId);

            TenantContextStack = TenantContextStack.Push(ctx);

            return ctx;
        }

        public void Dispose()
        {
            if (!TenantContextStack.IsEmpty)
            {
                TenantContextStack = TenantContextStack.Pop();
            }
        }
    }
}
