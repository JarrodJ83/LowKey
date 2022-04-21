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
            using var ctx = TenantIdContext.CreateFor(tenantId);
            var resolvedTenantId = await _ambientContextTenantIdResolver.Resolve();

            Assert.Equal(tenantId, resolvedTenantId);
        }

        [Theory, AutoData]
        public async Task Nested_Contexts_Allowed(TenantId rootTenant, TenantId nestedTenantId)
        {
            using var rootTenantCtx = TenantIdContext.CreateFor(rootTenant);
            var resolvedRootTenantId = await _ambientContextTenantIdResolver.Resolve();
            Assert.Equal(rootTenant, resolvedRootTenantId);
            Assert.NotNull(TenantIdContext.Current);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal(rootTenant, TenantIdContext.Current.TenantId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            using (var nestedTenantCtx = TenantIdContext.CreateFor(nestedTenantId))
            {
                var resolvedNestedTenantId = await _ambientContextTenantIdResolver.Resolve();
                Assert.Equal(nestedTenantId, resolvedNestedTenantId);
                Assert.NotNull(TenantIdContext.Current);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Assert.Equal(nestedTenantId, TenantIdContext.Current.TenantId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

            resolvedRootTenantId = await _ambientContextTenantIdResolver.Resolve();
            Assert.Equal(rootTenant, resolvedRootTenantId);
            Assert.NotNull(TenantIdContext.Current);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal(rootTenant, TenantIdContext.Current.TenantId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        [Fact]
        public Task Throwss_When_Context_Not_Set() =>
            Assert.ThrowsAsync<InvalidOperationException>(() => _ambientContextTenantIdResolver.Resolve());
        
    }
}
