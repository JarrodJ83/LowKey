using LowKey.Data.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class ActivityCommandSessionTests : IDisposable
    {
        DataStoreId TestDataStore = new("TestDataStore");
        Tenant TestTenant = new("TestDb", "test.server", 0);
        ICommandSession<TestClient> _session;
        IClientFactory<TestClient> _clientFactory;
        Activity? _commandActivity;
        ActivityListener _activityListener;

        public ActivityCommandSessionTests()
        {
            _clientFactory = new TestClientFactory();
            _session = new ActivityCommandSession<TestClient>(new Session<TestClient>(_clientFactory));
            _activityListener = new ActivityListener
            {
                ShouldListenTo = source => source.Name == ActivitySourceNames.CommandSessionActivityName,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                ActivityStarted = activity => _commandActivity = activity
            };
            ActivitySource.AddActivityListener(_activityListener);
        }

        [Fact]
        public async Task CommandActivityIsTraced()
        {
            await _session.Execute(TestDataStore, TestTenant, client => Task.CompletedTask);

            Assert.NotNull(_commandActivity);
            Assert.Equal(ActivitySourceNames.CommandSessionActivityName, _commandActivity?.Source.Name);
            Assert.Equal($"{nameof(ICommandSession<TestClient>.Execute)} {typeof(TestClient).FullName} Command", _commandActivity?.OperationName);
        }

        [Fact]
        public async Task CommandActivityHasCorrectTags()
        {
            await _session.Execute(TestDataStore, TestTenant, client => Task.CompletedTask);

            TestTag(OpenTelemetryDatabaseTags.DatabaseName, TestTenant.Name);
            TestTag(OpenTelemetryDatabaseTags.DatabaseServer, TestTenant.Server);
            TestTag(OpenTelemetryDatabaseTags.DatabasePort, TestTenant.Port.ToString());
            TestTag(OpenTelemetryDatabaseTags.DatabaseOperation, $"Command");
            TestTag(LowKeyDataActivityTags.ClientType, typeof(TestClient)?.FullName ?? string.Empty);
        }

        [Fact]
        public async Task CommandActivityShouldNotHaveTagsWhenActivityNotRequestingAllData()
        {
            _activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.PropagationData;

            await _session.Execute(TestDataStore, TestTenant, client => Task.CompletedTask);

            Assert.Empty(_commandActivity?.TagObjects);
        }

        void TestTag(string key, string expectedValue)
        {
            if (_commandActivity == null) throw new Exception("Activity null");

            var hasTag = _commandActivity.TagObjects.Any(tag => tag.Key.Equals(key));

            Assert.True(hasTag, $"Tag {key} was not found");

            KeyValuePair<string, object?> tag = _commandActivity.TagObjects.Single(tag => tag.Key.Equals(key));
            string actual = tag.Value?.ToString() ?? string.Empty;
            Assert.True(expectedValue.Equals(actual), $"Tag {key}\nExpected: {expectedValue}\nActual:  {actual}");
        }

        [Fact]
        public async Task ActivityIsPresentWithinExecutionOfCommand()
        {
            Activity? activityWithinCommand = Activity.Current;
            Assert.Null(activityWithinCommand);

            await _session.Execute(TestDataStore, TestTenant, async client => {
                await Task.Yield();

                activityWithinCommand = Activity.Current;
            });

            Assert.NotNull(activityWithinCommand);
        }

        public void Dispose()
        {
            _activityListener.Dispose();
            _commandActivity?.Dispose();
        }
    }
}
