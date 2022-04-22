using AutoFixture.Xunit2;
using LowKey.Data.Model;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class DataStoreTenantResolverTests
    {
        private readonly DataStoreTenantResolver _dataStoreTenantResolver;
        private readonly DataStoreTanantResolverRegistry _dataStoreTanantResolverRegistry;

        public DataStoreTenantResolverTests()
        {
            _dataStoreTanantResolverRegistry = new();
            _dataStoreTenantResolver = new(_dataStoreTanantResolverRegistry);
        }

        [Theory, AutoData]
        public async Task Correct_Tenant_Resolved(DataStoreId dataStoreId, Tenant tenant)
        {
            var testTenantResolver = new TestTenantResolver(dataStoreId, tenant);

            _dataStoreTanantResolverRegistry.RegisterTenantResolverFor(dataStoreId,
                () => testTenantResolver,
                () => testTenantResolver);

            var dataStoreTenant = await _dataStoreTenantResolver.Resolve(dataStoreId);

            Assert.Equal(tenant, dataStoreTenant);
        }
    }
}
