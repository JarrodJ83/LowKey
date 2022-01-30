using AutoFixture.Xunit2;
using LowKey.Data.Model;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class CommandSessionTests
    {
        CommandSession<TestClient> _session;
        Mock<ITenantedCommandSession<TestClient>> _tenantedCommandSession;
        DataStoreTanantResolverRegistry _dataStoreTenantResolverRegistry;
        DataStoreTenantResolver _dataStoreTenantResolver;

        public CommandSessionTests()
        {
            _dataStoreTenantResolverRegistry = new();
            _tenantedCommandSession = new();
            _dataStoreTenantResolver = new(_dataStoreTenantResolverRegistry);
            _session = new CommandSession<TestClient>(_tenantedCommandSession.Object, _dataStoreTenantResolver);
        }

        [Theory, AutoData]
        public async Task TenantRegisteredForDataStoreResolves(DataStoreId dataStoreId, Tenant tenant)
        {
            GivenTenantRegisteredForDataStore(dataStoreId, tenant);

            await WhenCommandIsExecuted(dataStoreId);

            ThenCommandWasRunAgainst(dataStoreId, tenant);
        }

        [Theory, AutoData]
        public Task NoTenantRegisteredForDataStoreThrows(DataStoreId dataStoreId)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => WhenCommandIsExecuted(dataStoreId));
        }


        private void ThenCommandWasRunAgainst(DataStoreId dataStoreId, Tenant tenant) => _tenantedCommandSession.Verify(session => session.Execute(
                                                                                            It.Is<DataStoreId>(d => d.Equals(dataStoreId)),
                                                                                            It.Is<Tenant>(t => t.Equals(tenant)),
                                                                                            It.IsAny<Func<TestClient, Task>>(),
                                                                                            It.IsAny<CancellationToken>()));

        private Task WhenCommandIsExecuted(DataStoreId dataStoreId) => _session.Execute(dataStoreId, testClient => Task.CompletedTask, CancellationToken.None);

        private void GivenTenantRegisteredForDataStore(DataStoreId dataStoreId, Tenant tenant) =>
            _dataStoreTenantResolverRegistry.RegisterTenantResolverFor(dataStoreId,
                cancel => Task.FromResult((ITenantResolver)new SingleTenantResolver(tenant)),
                cancel => Task.FromResult((ITenantIdResolver)new SingleTenantIdResolver(tenant.Id)));

        record TestResult;
    }
}