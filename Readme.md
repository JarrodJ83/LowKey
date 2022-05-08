# LowKey

Tired of having to build ways of getting configured clients for your data stores in each of your dotnet projects? Have a multi-tenant database configuration and need an easy way to get a configured client to your data stores? LowKey aims to be an unobtrusive way of creating pre-configured, connected clients to your different data stores (SQL, Redis, Postgres, etc) with multi-tenancy built in.

## Why would I use LowKey?

LowKey provides a simple way for registering your data stores for single or multi-tenancy and accessing them by name anywhere you need by injecting an instance of `IClientFactory`. This means you don't have to write any code for creating clients or dealing with multi-tenancy. LowKey will handle this for you.

A great example of how LowKey simplifies things is when you need different connections for reads and writes. Given this you can't simply just register your client with a DI framework and inject it as there would be no way to inject a read vs write connected client (at least not easily and clearly). At this point you're left with the option to write custom code to handle this or use LowKey. With LowKey you can, for example, register a client configured for your write instance with the name "write" and a client configured for your readonly instance with the name "read" and access them when needed using `IClientFactory`. 

## Key Design Points

* Core packages containing abstractions will have no dependencies outside of Microsoft/System
* Designed to be highly extensible to enable easily adding new Clients
* Support Multi-Tenancy

## Currently Supported

* SQL: LowKey.Data.Sql
* Postgres: LowKey.Data.Postgres

## Key Concepts

LowKey is made of a few basic concepts:

### Data Stores

In LowKey a DataStore represents an external storage system you'd like to connect to. It's represented by an Id which is used to reference it when establishing a connection and a Name that's to represent the actual store. For example, if you had a SQL database named "App" it could be represented as `new DataStore("sql-app", "app")`

### Tenants

In LowKey a Tenant represents the location of a DataStore. It has an Id which is used to look it up along with a Server and a Port.

### Client Factory

DataStores can be registered as single or multi-tenant. When executing LowKey will use the configuration to find the Tenant for the DataStore and then use them both to build a connected client of your choice.

`IClientFactory` is what glues everything together. This interface will allow you to get a pre-cofnigured client for any of your registered Data Stores and will automatically resolve the corrent Tenant based on configuration.

## Examples

### Single Tenant

Below is an example of configuring and using a single tenanted connection to a SQL Database.

#### Configuration
This configuration will connect to the `app` database on `localhost` using the supplied `SqlConnectionStringBuilder` by replacing the builder's `InitialCatalog` and `Server` at runtime.

```
services.AddLowKeyData(config => 
{
        config.AddStore(new DataStore(id: "sql", name: "app"), server: "localhost")
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
This configuration will connect to the `app` database on `localhost` using the supplied `SqlConnectionStringBuilder` by replacing the builder's `InitialCatalog` and `Server` at runtime from the Tenant looked up by the using the supplied `ITenantIdResolver` and `ITenantResolver`.

```
services.AddLowKeyData(config => 
{
        var tenantResolver = new QueryStringTenantResolver(new Tenant(server, server));
        config.AddStore(new DataStore("sql-multi-tenant", "app"),
                tenantResolverFactory: () => tenantResolver,
                tenantIdResolverFactory: () => tenantResolver)
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
