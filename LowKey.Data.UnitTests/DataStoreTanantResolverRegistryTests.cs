using AutoFixture.Xunit2;
using LowKey.Data.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class DataStoreTanantResolverRegistryTests
    {
        private readonly DataStoreTanantResolverRegistry _dataStoreTanantResolverRegistry;

        public DataStoreTanantResolverRegistryTests()
        {
            _dataStoreTanantResolverRegistry = new DataStoreTanantResolverRegistry();
        }

        [Theory, AutoData]
        public async Task Returns_Registered_TenantIdResolver(DataStoreId dataStoreId, TestTenantResolver testTenantResolver)
        {
            _dataStoreTanantResolverRegistry.RegisterTenantResolverFor(dataStoreId,
                cancel => Task.FromResult((ITenantResolver)testTenantResolver),
                cancel => Task.FromResult((ITenantIdResolver)testTenantResolver));

            var resolvedTenantIdResolver = await _dataStoreTanantResolverRegistry.GetTenantIdResolverFor(dataStoreId);
            Assert.Equal(testTenantResolver, resolvedTenantIdResolver);

            var resolvedTenantResolver = await _dataStoreTanantResolverRegistry.GetTenantResolverFor(dataStoreId);
            Assert.Equal(testTenantResolver, resolvedTenantResolver);
        }

        [Theory, AutoData]
        public async Task Throws_InvalidOperationException_If_Resolvers_NotFound(DataStoreId dataStoreId)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _dataStoreTanantResolverRegistry.GetTenantIdResolverFor(dataStoreId));
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _dataStoreTanantResolverRegistry.GetTenantResolverFor(dataStoreId));
        }
    }

    public class TestTenantResolver : ITenantResolver, ITenantIdResolver
    {
        public Task<Tenant> Resolve(DataStoreId dataStoreId, TenantId tenantId, CancellationToken cancel = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<TenantId> Resolve()
        {
            throw new System.NotImplementedException();
        }
    }
}
