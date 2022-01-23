using AutoFixture.Xunit2;
using LowKey.Data.Model;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class QuerySessionTests
    {
        QuerySession<TestClient> _session;
        Mock<ITenantedQuerySession<TestClient>> _tenantedQuerySession;
        DataStoreTanantResolverRegistry _dataStoreTenantResolverRegistry;
        DataStoreTenantResolver _dataStoreTenantResolver;

        public QuerySessionTests()
        {
            _dataStoreTenantResolverRegistry = new();
            _tenantedQuerySession = new();
            _dataStoreTenantResolver = new DataStoreTenantResolver(_dataStoreTenantResolverRegistry);
            _session = new QuerySession<TestClient>(_tenantedQuerySession.Object, _dataStoreTenantResolver);
        }

        [Theory, AutoData]
        public async Task TenantRegisteredForDataStoreResolves(DataStoreId dataStoreId, Tenant tenant)
        {
            GivenTenantRegisteredForDataStore(dataStoreId, tenant);

            await WhenQueryIsExecuted(dataStoreId);

            ThenQueryWasRunAgainst<TestResult>(dataStoreId, tenant);
        }

        [Theory, AutoData]
        public Task NoTenantRegisteredForDataStoreThrows(DataStoreId dataStoreId)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => WhenQueryIsExecuted(dataStoreId));
        }


        private void ThenQueryWasRunAgainst<T>(DataStoreId dataStoreId, Tenant tenant) => _tenantedQuerySession.Verify(session => session.Execute(
                                                                                            It.Is<DataStoreId>(d => d.Equals(dataStoreId)),
                                                                                            It.Is<Tenant>(t => t.Equals(tenant)),
                                                                                            It.IsAny<Func<TestClient, Task<T>>>(),
                                                                                            It.IsAny<CancellationToken>()));

        private Task WhenQueryIsExecuted(DataStoreId dataStoreId) => _session.Execute(dataStoreId, testClient => Task.FromResult(new TestResult()));

        private void GivenTenantRegisteredForDataStore(DataStoreId dataStoreId, Tenant tenant) =>
            _dataStoreTenantResolverRegistry.RegisterTenantResolverFor(dataStoreId, cancel => Task.FromResult((ITenantResolver)new SingleTenantResolver(tenant)),
                cancel => Task.FromResult((ITenantIdResolver)new SingleTenantIdResolver(tenant.Id)));

        record TestResult;
    }
}