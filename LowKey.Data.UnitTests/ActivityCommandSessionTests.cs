using LowKey.Data.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public partial class ActivityCommandSessionTests
    {
        Db TestDb = new("TestDb", "test.server", 0);
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
                ShouldListenTo = _ => true,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                ActivityStarted = activity => _commandActivity = activity
            };
            ActivitySource.AddActivityListener(_activityListener);
        }

        [Fact]
        public async Task CommandActivityIsTraced()
        {
            await _session.Execute(TestDb, client => {
                return Task.CompletedTask;
            });

            Assert.NotNull(_commandActivity);
            Assert.Equal(ActivitySourceNames.SessionActivityName, _commandActivity?.Source.Name);
            Assert.Equal($"{nameof(ICommandSession<TestClient>.Execute)} {typeof(TestClient).FullName} Command", _commandActivity?.OperationName);
        }

        [Fact]
        public async Task CommandActivityHasCorrectTags()
        {
            await _session.Execute(TestDb, client => {
                return Task.CompletedTask;
            });

            TestTag(OpenTelemetryDatabaseTags.DatabaseName, TestDb.Name);
            TestTag(OpenTelemetryDatabaseTags.DatabaseServer, TestDb.Server);
            TestTag(OpenTelemetryDatabaseTags.DatabasePort, TestDb.Port.ToString());
            TestTag(OpenTelemetryDatabaseTags.DatabaseOperation, $"Command");
        }

        [Fact]
        public async Task CommandActivityShouldNotHaveTagsWhenActivityNotRequestingAllData()
        {
            _activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.PropagationData;

            await _session.Execute(TestDb, client => {
                return Task.CompletedTask;
            });

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
    }
}
