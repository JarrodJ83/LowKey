using LowKey.Data.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data.Example.Net5WebApp
{
    public class QueryStringTenantResolver : ITenantResolver, ITenantIdResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Tenant _defaultTenant;
        public QueryStringTenantResolver(Tenant defaultTenant)
        {
            _httpContextAccessor = new HttpContextAccessor();
            _defaultTenant = defaultTenant;
        }

        public Task<Tenant> Resolve(DataStoreId dataStoreId, TenantId tenantId, CancellationToken cancel = default) => Task.FromResult(GetTenant());

        public Task<TenantId> Resolve() => Task.FromResult(GetTenant().Id);

        Tenant GetTenant()
        {
            if(_httpContextAccessor.HttpContext.Request.Query.TryGetValue("tenant", out var tenantId))
            {
                return new Tenant(tenantId, tenantId);
            }

            return _defaultTenant;
        }
    }
}
