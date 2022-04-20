using AutoFixture.Xunit2;
using LowKey.Data.Model;
using LowKey.Data.MultiTenancy;
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
        public async Task ResolvedTenantIsCorrect_SingleDataStoreWithSingleTenant(DataStoreId dataStoreId, string server, int port)
        {
            _services.AddLowKeyData(lowKey =>
            {
                lowKey.AddStore(dataStoreId.Value, server, port);
            });

            var tenantResolver = await GetTenantResolverFor(dataStoreId);

            Assert.IsType<SingleTenantResolver>(tenantResolver);

            var tenant = await tenantResolver.Resolve(dataStoreId, new TenantId(server));
            Assert.Equal(server, tenant.Server);
            Assert.Equal(port, tenant.Port);
        }

        [Theory, AutoData]
        public async Task ResolvedTenantsAreCorrect_SingleDataStoreWithMulitpleTenants(DataStoreId dataStoreId, Tenant[] tenants)
        {
            var dataStoreTenants = new Dictionary<DataStoreId, Tenant[]> { { dataStoreId, tenants } };
            _services.AddLowKeyData(lowKey =>
            {
                lowKey.AddStore(dataStoreId.Value,
                    cancel => Task.FromResult((ITenantResolver)new InMemoryTenantResolver(dataStoreTenants)),
                    cancel => Task.FromResult((ITenantIdResolver)new AmbientContextTenantIdResolver()));
            });

            var tenantResolver = await GetTenantResolverFor(dataStoreId);

            Assert.IsType<InMemoryTenantResolver>(tenantResolver);

            foreach (var expectedTenant in tenants)
            {
                var tenant = await tenantResolver.Resolve(dataStoreId, expectedTenant.Id);
                Assert.Equal(expectedTenant, tenant);
            }
        }

        [Theory, AutoData]
        public async Task ClientCreatedForCorrectStoreAndTenant_SingleDataStoreWithMulitpleTenants(DataStoreId dataStoreId, Tenant[] tenants)
        {
            var dataStoreTenants = new Dictionary<DataStoreId, Tenant[]> { { dataStoreId, tenants } };
            _services.AddLowKeyData(lowKey =>
            {
                lowKey.AddStore(dataStoreId.Value,
                    cancel => Task.FromResult((ITenantResolver)new InMemoryTenantResolver(dataStoreTenants)),
                    cancel => Task.FromResult((ITenantIdResolver)new AmbientContextTenantIdResolver()))
                    .WithTestClient();
            });

            var clientFactory = _services.BuildServiceProvider().GetService<IClientFactory>();
            Assert.NotNull(clientFactory);
            foreach (var tenant in tenants)
            {
                using var tenantContext = TenantContext.CreateFor(tenant.Id);
                var client = await clientFactory.Create<TestClient>(dataStoreId);

                Assert.Equal(tenant, client.Tenant);
                Assert.Equal(dataStoreId, client.DataStore.Id);
            }
        }


        [Theory, AutoData]
        public async Task ClientCreatedForCorrectStoreAndTenant_SingleDataStoreWithSingleTenant(DataStoreId dataStoreId, Tenant tenant)
        {
            var dataStoreTenants = new Dictionary<DataStoreId, Tenant[]> { { dataStoreId, new[] { tenant } } };
            _services.AddLowKeyData(lowKey =>
            {
                lowKey.AddStore(dataStoreId.Value,
                    cancel => Task.FromResult((ITenantResolver)new InMemoryTenantResolver(dataStoreTenants)),
                    cancel => Task.FromResult((ITenantIdResolver)new AmbientContextTenantIdResolver()))
                    .WithTestClient();
            });

            var clientFactory = _services.BuildServiceProvider().GetService<IClientFactory>();
            Assert.NotNull(clientFactory);

            using var tenantContext = TenantContext.CreateFor(tenant.Id);
            var client = await clientFactory.Create<TestClient>(dataStoreId);

            Assert.Equal(tenant, client.Tenant);
            Assert.Equal(dataStoreId, client.DataStore.Id);
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
