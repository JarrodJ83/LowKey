using AutoFixture.Xunit2;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class SessionTests
    {
        DataStoreId TestDataStore = new DataStoreId("Test");
        Session<TestClient> _session;
        Mock<ITenantedQuerySession<TestClient>> _tenantedQuerySession;
        DataStoreTanantResolverRegistry _dataStoreTenantResolverRegistry;

        public SessionTests()
        {
            _dataStoreTenantResolverRegistry = new DataStoreTanantResolverRegistry();
            _tenantedQuerySession = new Mock<ITenantedQuerySession<TestClient>>();
            _session = new Session<TestClient>(_tenantedQuerySession.Object, _dataStoreTenantResolverRegistry);
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
            _dataStoreTenantResolverRegistry.RegisterTenantResolverFor(dataStoreId, cancel => Task.FromResult((ITenantResolver)new TestTenantResolver(tenant)));

        record TestResult;

        class TestTenantResolver : ITenantResolver
        {
            private readonly Tenant _tenant;
            public TestTenantResolver(Tenant tenant)
            {
                _tenant = tenant;
            }

            public Task<Tenant> Resolve(DataStoreId dataStoreId) => Task.FromResult(_tenant);
        }
    }
}