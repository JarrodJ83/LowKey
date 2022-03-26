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

In LowKey a DataStore represents an external storage system you'd like to connect to. It's represented by an Id which is used to reference it when establishing a connection and a Name that's to represent the actual store. For example, the Master database of a SQL server might be represented as `new DataStore("sql-master", "master")`

### Tenants

In LowKey a Tenant represents the location of a DataStore. It has an Id which is used to look it up along with a Server and a Port.

### Client Factory

DataStores can be registered as single or multi-tenant. When executing LowKey will use the configuration to find the Tenant for the DataStore and then use them both to build a connected client of your choice.

`IClientFactory` is what glues everything together. This interface will allow you to get a pre-cofnigured client for any of your registered Data Stores and will automatically resolve the corrent Tenant based on configuration.

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
        private readonly IClientFactory _clientFactory;

        public IndexModel(ILogger<IndexModel> logger, IClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task OnGet()
        {
            var dataStoreId = new DataStoreId("sql");
            DbConnection client = await _clientFactory.Resolve<DbConnection>(dataStoreId);
            int result = await client.QueryFirstAsync<int>("select 1");
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
         private readonly IClientFactory _clientFactory;

        public IndexModel(ILogger<IndexModel> logger, IClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task OnGet()
        {
            var dataStoreId = new DataStoreId("sql-multi-tenant");
            DbConnection client = await _clientFactory.Resolve<DbConnection>(dataStoreId);
            int result = await client.QueryFirstAsync<int>("select 1");
        }
}
```
