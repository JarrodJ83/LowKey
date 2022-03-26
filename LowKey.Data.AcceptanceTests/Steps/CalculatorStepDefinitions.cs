using LowKey.Data.Model;
using LowKey.Data.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Shouldly;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace LowKey.Data.AcceptanceTests.Steps
{
    [Binding]
    public sealed class CalculatorStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private IHost? _host;
        private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder;
        private readonly IConfiguration _config;
        private ICommandSession<DbConnection>? _commandSession;

        public CalculatorStepDefinitions(ScenarioContext scenarioContext)
        {            
            _scenarioContext = scenarioContext;
            _config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            _sqlConnectionStringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder
            {
                UserID = _config.GetValue<string>("SQL_USERNAME"),
                Password = _config.GetValue<string>("SQL_PASSWORD")
            };
        }

        [Given(@"a ""(.*)"" datastore for database ""(.*)""")]
        public void GivenADatastore(string datastoreId, string database)
        {
            _scenarioContext.Set(new DataStore(datastoreId, database));
        }

        [Given(@"a single SQL Server tenant")]
        public void GivenASingleTenantOnServerAndPort()
        {
            var server = _config.GetValue<string>("SQL_SERVER");

            _scenarioContext.Set(new SqlServerTenant(server));
        }

        [When(@"a command is executed")]
        public void WhenACommandIsExecuted()
        {
            var dataStore = _scenarioContext.Get<DataStore>();
            var tenant = _scenarioContext.Get<SqlServerTenant>();
            _host = Host.CreateDefaultBuilder().ConfigureServices(services =>
            {
                services.AddLowKeyData(config =>
                {
                    config.AddStore(dataStore, tenant).WithSqlServer(_sqlConnectionStringBuilder);
                });
            }).Build();

            _commandSession = _host.Services.GetService(typeof(ICommandSession<DbConnection>)) as ICommandSession<DbConnection>;
        }

        [Then(@"then it should be executed on ""(.*)"" data store")]
        public Task ThenThenItShouldBeForDataStore(string dataStoreId)
        {
            var dataStore = _scenarioContext.Get<DataStore>();
            
            var tenant = _scenarioContext.Get<Tenant>();
            
            return _commandSession.Execute(dataStore.Id, conn =>
            {
                var sqlConnBuilder = new SqlConnectionStringBuilder(conn.ConnectionString);

                sqlConnBuilder.InitialCatalog.ShouldBe(dataStore.Name);
                sqlConnBuilder.DataSource.ShouldBe($"{tenant.Server}:{tenant.Port}");

                return Task.CompletedTask;
            });
        }
    }
}
