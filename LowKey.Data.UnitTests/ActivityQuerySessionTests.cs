using LowKey.Data.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class ActivityQuerySessionTests
    {
        Db TestDb = new("TestDb", "test.server", 0);
        IQuerySession<TestClient> _session;
        IClientFactory<TestClient> _clientFactory;
        Activity? _activity;
        ActivityListener _activityListener;

        public ActivityQuerySessionTests()
        {
            _clientFactory = new TestClientFactory();
            _session = new ActivityQuerySession<TestClient>(new Session<TestClient>(_clientFactory));
            _activityListener = new ActivityListener
            {
                ShouldListenTo = _ => true,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                ActivityStarted = activity => _activity = activity
            };
            ActivitySource.AddActivityListener(_activityListener);
        }

        [Fact]
        public async Task QueryActivityIsTraced()
        {
            await _session.Execute(TestDb, client => Task.FromResult(string.Empty));

            Assert.NotNull(_activity);
            Assert.Equal(ActivitySourceNames.SessionActivityName, _activity?.Source.Name);
            Assert.Equal($"{nameof(ICommandSession<TestClient>.Execute)} {typeof(TestClient).FullName} Query", _activity?.OperationName);
        }

        [Fact]
        public async Task QueryActivityHasCorrectTags()
        {
            await _session.Execute(TestDb, client => Task.FromResult(string.Empty));

            TestTag(OpenTelemetryDatabaseTags.DatabaseName, TestDb.Name);
            TestTag(OpenTelemetryDatabaseTags.DatabaseServer, TestDb.Server);
            TestTag(OpenTelemetryDatabaseTags.DatabasePort, TestDb.Port.ToString());
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

            await _session.Execute(TestDb, client => Task.FromResult(string.Empty));

            Assert.Empty(_activity?.TagObjects);
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
    }
}
