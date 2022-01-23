using LowKey.Data.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class ActivityTenantedQuerySessionTests : IDisposable
    {
        DataStoreId TestDataStore = new("Test");
        Tenant TestTenant = new("TestDb", "test.server", 0);
        ITenantedQuerySession<TestClient> _session;
        IClientFactory<TestClient> _clientFactory;
        Activity? _activity;
        ActivityListener _activityListener;

        public ActivityTenantedQuerySessionTests()
        {
            _clientFactory = new TestClientFactory();
            var clientFactoryRegistry = new DataStoreClientFactoryRegistry();
            clientFactoryRegistry.RegisterClientFor(TestDataStore, cancel => Task.FromResult(_clientFactory));            

            _session = new ActivityTenantedQuerySession<TestClient>(new TenantedSession<TestClient>(clientFactoryRegistry));
            _activityListener = new ActivityListener
            {
                ShouldListenTo = source => source.Name.Equals(ActivitySourceNames.QuerySessionActivityName),
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                ActivityStarted = activity => _activity = activity
            };
            ActivitySource.AddActivityListener(_activityListener);
        }

        [Fact]
        public async Task QueryActivityIsTraced()
        {
            await _session.Execute(TestDataStore, TestTenant, client => Task.FromResult(string.Empty));

            Assert.NotNull(_activity);
            Assert.Equal(ActivitySourceNames.QuerySessionActivityName, _activity?.Source.Name);
            Assert.Equal($"{nameof(ITenantedCommandSession<TestClient>.Execute)} {typeof(TestClient).FullName} Query", _activity?.OperationName);
        }

        [Fact]
        public async Task QueryActivityHasCorrectTags()
        {
            await _session.Execute(TestDataStore, TestTenant, client => Task.FromResult(string.Empty));

            TestTag(OpenTelemetryDatabaseTags.DatabaseName, TestTenant.Id.Value);
            TestTag(OpenTelemetryDatabaseTags.DatabaseServer, TestTenant.Server);
            TestTag(OpenTelemetryDatabaseTags.DatabasePort, TestTenant.Port?.ToString() ?? string.Empty);
            TestTag(OpenTelemetryDatabaseTags.DatabaseOperation, "Query");
#pragma warning disable CS8604 // Possible null reference argument.
            TestTag(LowKeyDataActivityTags.ClientType, typeof(TestClient)?.FullName);
            TestTag(LowKeyDataActivityTags.QueryResultType, typeof(string)?.FullName);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        [Fact]
        public async Task QueryActivityShouldNotHaveTagsWhenActivityNotRequestingAllData()
        {
            _activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.PropagationData;

            await _session.Execute(TestDataStore, TestTenant, client => Task.FromResult(string.Empty));

            Assert.Empty(_activity?.TagObjects);
        }

        [Fact]
        public async Task ActivityIsPresentWithinExecutionOfQuery()
        {
            Activity? activityWithinQuery = Activity.Current;
            Assert.Null(activityWithinQuery);

            await _session.Execute(TestDataStore, TestTenant, async client => {
                await Task.Yield();

                activityWithinQuery = Activity.Current;
                
                return Task.FromResult(string.Empty); 
            });

            Assert.NotNull(activityWithinQuery);
        }

        void TestTag(string key, string expectedValue)
        {
            if (_activity == null) throw new Exception("Activity null");

            var hasTag = _activity.TagObjects.Any(tag => tag.Key.Equals(key));

            Assert.True(hasTag, $"Tag {key} was not found");

            KeyValuePair<string, object?> tag = _activity.TagObjects.Single(tag => tag.Key.Equals(key));
            string actual = tag.Value?.ToString() ?? string.Empty;
            Assert.True(expectedValue.Equals(actual), $"Tag {key}\nExpected: {expectedValue}\nActual:  {actual}");
        }

        public void Dispose()
        {
            _activity?.Dispose();
            _activityListener.Dispose();
        }
    }
}
