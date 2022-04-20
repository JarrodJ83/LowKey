using AutoFixture.Xunit2;
using LowKey.Data.Model;
using LowKey.Data.MultiTenancy;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class AmbientContextTenantIdResolverTests
    {
        AmbientContextTenantIdResolver _ambientContextTenantIdResolver;

        public AmbientContextTenantIdResolverTests()
        {
            _ambientContextTenantIdResolver = new AmbientContextTenantIdResolver();
        }

        [Theory, AutoData]
        public async Task Returns_Correct_TenantId_When_Context_Set(TenantId tenantId)
        {
            using var ctx = TenantContext.CreateFor(tenantId);
            var resolvedTenantId = await _ambientContextTenantIdResolver.Resolve();

            Assert.Equal(tenantId, resolvedTenantId);
        }

        [Fact]
        public Task Throwss_When_Context_Not_Set() =>
            Assert.ThrowsAsync<InvalidOperationException>(() => _ambientContextTenantIdResolver.Resolve());
        
    }
}
