using AutoFixture.Xunit2;
using LowKey.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
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
        public async Task SingleDataStoreWithSingleTenant(DataStoreId dataStoreId, string server, int port)
        {
            _services.AddLowKeyData(lowKey =>
            {
                lowKey.AddStore(dataStoreId.Name, server, port);
            });

            var tenantResolver = await GetTenantResolverFor(dataStoreId);

            Assert.IsType<SingleTenantResolver>(tenantResolver);

            var tenant = await tenantResolver.Resolve(dataStoreId);
            Assert.Equal(server, tenant.Server);
            Assert.Equal(port, tenant.Port);
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
