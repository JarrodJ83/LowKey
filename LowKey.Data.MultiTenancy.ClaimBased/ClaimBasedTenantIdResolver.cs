using LowKey.Data.Model;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LowKey.Data.MultiTenancy.Jwt
{
    public class ClaimBasedTenantIdResolver : ITenantIdResolver
    {
        public class Settings
        {
            public string TenantIdClaimName { get; }

            public Settings(string tenantIdClaimName)
            {
                TenantIdClaimName = tenantIdClaimName;
            }
        }

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Settings _settings;

        public ClaimBasedTenantIdResolver(IHttpContextAccessor httpContextAccessor, Settings settings)
        {
            _settings = settings;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<TenantId> Resolve()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null) throw new InvalidOperationException("No user could be found to resolve TenantId from.");

            Claim? tenantIdClaim = user.Claims.SingleOrDefault(claim => claim.Type.Equals(_settings.TenantIdClaimName));

            if (tenantIdClaim == null) throw new InvalidOperationException($"No claim with type \"{_settings.TenantIdClaimName}\" could be found");

            return Task.FromResult(new TenantId(tenantIdClaim.Value));
        }
    }
}