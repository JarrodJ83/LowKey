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
        
        public ActivityCommandSessionTests()
        {
            _clientFactory = new TestClientFactory();
            _session = new ActivityCommandSession<TestClient>(new Session<TestClient>(_clientFactory));
            ActivitySource.AddActivityListener(new ActivityListener
            {
                ShouldListenTo = _ => true,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                ActivityStarted = activity => _commandActivity = activity
            });
        }

        [Fact]
        public async Task CommandActivityIsTraced()
        {
            await _session.Execute(TestDb, client => {
                return Task.CompletedTask;
            });

            Assert.NotNull(_commandActivity);
            Assert.Equal(ActivitySourceNames.SessionActivityName, _commandActivity?.Source.Name);
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
            TestTag(OpenTelemetryDatabaseTags.DatabaseOperation, $"{nameof(ICommandSession<TestClient>.Execute)} {typeof(TestClient).FullName} Command");
        }

        void TestTag(string key, string expectedValue)
        {
            if (_commandActivity == null) throw new Exception("Activity null");

            var hasTag = _commandActivity.Tags.Any(tag => tag.Key.Equals(key));

            Assert.True(hasTag, $"Tag {key} was not found");

            KeyValuePair<string, string?> tag = _commandActivity.Tags.Single(tag => tag.Key.Equals(key));
            var actual = tag.Value ?? string.Empty;
            Assert.True(expectedValue.Equals(actual), $"Tag {key}\nExpected: {expectedValue}\nActual:  {actual}");
        }
    }
}
