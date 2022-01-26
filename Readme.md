# LowKey

LowKey aims to be an unobtrusive way of creating pre-configured, connected clients to your different data stores (SQL, Redis, Postgres, etc)

## Key Design Points

* Core packages containing abstractions will have no dependencies outside of Microsoft/System
* Designed to be highly extensible to enable easily adding new Clients
* Support Multi-Tenancy

## Currently Supported

* SQL: LowKey.Data.Sql

## Key Concepts

LowKey is made of a few basic concepts:

### Data Stores

In LowKey a DataStore represents a data store you'd like to connect to. It's represented by an Id which is used to reference it when establishing a connection and a Name that's to represent the actual store. For example, the Master database of a SQL server might be represented as `new DataStore("sql-master", "master")`

### Tenants

In LowKey a Tenant represents the location of a DataStore. It has an Id which is used to look it up along with a Server and a Port.

### Commands vs Queries

LowKey separates its interfaces into Commands and Queries to allow for adding appropriate cross cutting concerns. Both tenanted and non-tenanted interfaces will each have separate interfaces for commands and queries:

### Putting it Together

DataStores can be registered as single or multi-tenant. When executing LowKey will use the configuration to find the Tenant for the DataStore and then use them both to build a connected client of your choice.

## Examples

### Single Tenant

Below is an example of configuring and using a single tenanted connection to a SQL Database.

#### Configuration
This configuration will connect to the `master` database on `localhost` using the supplied `SqlConnectionStringBuilder` by replacing the builder's `InitialCatalog` and `Server` at runtime.

```
services.AddLowKeyData(config => 
{
        config.AddStore(new DataStore(id: "sql", name: "master"), server: "localhost")
                .WithSqlServer(new SqlConnectionStringBuilder
                {
                UserID = Configuration.GetValue<string>("SQL_USERNAME"),
                Password = Configuration.GetValue<string>("SQL_PASSWORD")
                });
});
```

#### Usage
The below example uses Dapper to run a SQL query and command.
```
public class IndexModel : PageModel
{
        private readonly IQuerySession<DbConnection> _qrySession;
        private readonly ICommandSession<DbConnection> _cmdSession;

        public IndexModel(ILogger<IndexModel> logger, IQuerySession<DbConnection> qrySession, ICommandSession<DbConnection> cmdSession)
        {
                _qrySession = qrySession;
                _cmdSession = cmdSession;
        }

        public async Task OnGet()
        {
                var dataStoreId = new DataStoreId("sql");
                await _cmdSession.Execute<DbConnection>(dataStoreId, conn =>
                        conn.ExecuteAsync("select 1"));
                var result = await _qrySession.Execute(dataStoreId, conn =>
                        conn.QueryAsync("select 1"));
        }
}
```

### Multi-Tenant

Below is an example of configuring and using a multi-tenant configuratoin to a SQL Server  Database. In order to leverage multi-tenancy LowKey needs to know how to resolve two things: The `TenantId` and the actual `Tenant`. The `TenantId` will likely come from something like Jwt for the authenticated user. This is the ID used to lookup the Tenant from your system. You must implement and provide two interfaces to tell LowKey how to obtain this data: `ITenantIdResolver` and `ITenantResolver`.

#### Configuration
This configuration will connect to the `master` database on `localhost` using the supplied `SqlConnectionStringBuilder` by replacing the builder's `InitialCatalog` and `Server` at runtime from the Tenant looked up by the using the supplied `ITenantIdResolver` and `ITenantResolver`.

```
services.AddLowKeyData(config => 
{
        var tenantResolver = new QueryStringTenantResolver(new Tenant(server, server));
        config.AddStore(new DataStore("sql-multi-tenant", "master"),
                tenantResolverFactory: cancel => Task.FromResult((ITenantResolver)tenantResolver),
                tenantIdResolverFactory: cancel => Task.FromResult((ITenantIdResolver)tenantResolver))
                        .WithSqlServer(new SqlConnectionStringBuilder
                        {
                                UserID = Configuration.GetValue<string>("SQL_USERNAME"),
                                Password = Configuration.GetValue<string>("SQL_PASSWORD")
                        });
});
```

#### Usage
The below example uses Dapper to run a SQL query and command. You'll notice we're using the same interfaces for Multi-Tenancy support as single. The only difference is we're using a different `DataStoreId` to tell it to use the store that is configured for Multi-Tenancy.

```
public class IndexModel : PageModel
{
        private readonly IQuerySession<DbConnection> _qrySession;
        private readonly ICommandSession<DbConnection> _cmdSession;

        public IndexModel(ILogger<IndexModel> logger, IQuerySession<DbConnection> qrySession, ICommandSession<DbConnection> cmdSession)
        {
                _qrySession = qrySession;
                _cmdSession = cmdSession;
        }

        public async Task OnGet()
        {
                var dataStoreId = new DataStoreId("sql-multi-tenant");
                await _cmdSession.Execute<DbConnection>(dataStoreId, conn =>
                        conn.ExecuteAsync("select 1"));
                var result = await _qrySession.Execute(dataStoreId, conn =>
                        conn.QueryAsync("select 1"));
        }
}
```
