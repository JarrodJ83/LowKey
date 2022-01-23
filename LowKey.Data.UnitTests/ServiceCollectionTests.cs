using AutoFixture.Xunit2;
using LowKey.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class ServiceCollectionTests
    {
        private IServiceCollection _services;

        public ServiceCollectionTests()
        {
            _services = new ServiceCollection();
        }

        [Theory, AutoData]
        public async Task SingleDataStoreWithSingleTenant(DataStoreId dataStoreId, string server, TenantId tenantId, int port)
        {
            _services.AddLowKeyData(lowKey =>
            {
                lowKey.AddStore(dataStoreId.Value, server, tenantId.Value, port);
            });

            var tenantResolver = await GetTenantResolverFor(dataStoreId);

            Assert.IsType<SingleTenantResolver>(tenantResolver);

            var tenant = await tenantResolver.Resolve(dataStoreId, tenantId);
            Assert.Equal(server, tenant.Server);
            Assert.Equal(port, tenant.Port);
        }

        [Theory, AutoData]
        public async Task SingleDataStoreWithMulitpleTenants(DataStoreId dataStoreId, Tenant[] tenants)
        {
            var dataStoreTenants = new Dictionary<DataStoreId, Tenant[]> { { dataStoreId, tenants } };
            _services.AddLowKeyData(lowKey =>
            {
                lowKey.AddStore(dataStoreId.Value, cancel => Task.FromResult((ITenantResolver)new InMemoryTenantResolver(dataStoreTenants)));
            });

            var tenantResolver = await GetTenantResolverFor(dataStoreId);

            Assert.IsType<InMemoryTenantResolver>(tenantResolver);

            foreach (var expectedTenant in tenants)
            {
                var tenant = await tenantResolver.Resolve(dataStoreId, expectedTenant.Id);
                Assert.Equal(expectedTenant, tenant);
            }
        }

        Task<ITenantResolver> GetTenantResolverFor(DataStoreId dataStoreId) =>
            GetConfig().DataStoreTanantResolverRegistry.GetTenantResolverFor(dataStoreId);

        LowKeyConfiguration GetConfig()
        {
            var config = _services.BuildServiceProvider().GetService<LowKeyConfiguration>();
            Assert.NotNull(config);
            return config ?? throw new ArgumentNullException(nameof(LowKeyConfiguration));
        }
    }
}
