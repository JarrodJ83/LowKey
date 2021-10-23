# LowKey
LowKey aims to be an unobtrusive, highly extensible set of building blocks for building dotnet APIs and Services. This includes abstractions for interacting with data stores, sending and recieving events, and implementing basic CQRS.

## Key Design Points
* Core packages containing abstractions will have no dependencies
* SOLID principles will be followed to allow for a highly maintainable and extensible set of libraries
* Avoid making consumers "sprinkle" custom attributes and interfaces throughout their own codebase in order to leverage LowKey (crucial to lightweight and unobtrusive design). A consumer's business code should only need to rely on the core abstractions. (Thanks to [Jeremy D. Miller](https://jeremydmiller.com/) and his [JasperFx](https://jasperfx.github.io) for inspiration here). The host startup code will configure the implementations and features of LowKey.
* Avoid reflection aside from in service registrations

## LowKey Data
Building blocks for interacting with data stores without worrying about the details. Just indicate the Data Store (Database) and the how you'd like to interact with it (DbConnection, EFCore, Marten, etc) and you'll get back a pre-configured connection ready to use. For example:

### Using a DbConnection w/ Dapper

```
private readonly IDataStore<DbConnection> _dataStore;
private readonly ProductDatabase _productDb;

public async Task<Product?> Handle(ProductById query, CancellationToken) =>
     _dataStore.QueryAsync(_productDb, conn =>        
        conn.QuerySingleOrDefaultAsync<Product>("select id, name from product where id = @id", query));
```

### Using a EFCore

```
private readonly IDataStore<ProductDbContext> _dataStore;
private readonly ProductDatabase _productDb;

...

public async Task<Product?> Handle(ProductById query, CancellationToken) =>
     _dataStore.QueryAsync(_productDb, ctx =>        
        ctx.Products.SingleOrDefault(product => product.Id == query.Id));
```

### Using a EFCore Multi-Tenanted

```
private readonly IDataStore<ProductDbContext> _dataStore;
private readonly ProductDatabase _productDb; 

...

public async Task<Product?> Handle(ProductById query, CancellationToken) =>
     _dataStore.QueryAsync(query.TenantId, _productDb, ctx =>        
        ctx.Products.SingleOrDefault(product => product.Id == query.Id));
```

## LowKey CQRS
Building blocks for implementing CQRS to allow easy cross cutting concerns to be implemented. Notice that unlike other libraries there are no empty IQuery/ICommand interfaces or attributes that need added to your query or commands in order for them to work with LowKey.

### Commands
Command are for handling state change in your application. In pure theory a "command" never returns a result but in the real world often a command creates some type of entity that you'd also like to return to the caller so there are interfaces for both.

```
ICommandHandler<TCmd>
ICommandHandler<TCmd, TResult>
```

### Queries
Queries represent read-only operations to collect data. They should never mutate state.

```
IQueryHandler<TQry, TResult>
```

### Requests
Requests are a higher-level abstraction that could represent a web-request or maybe a request disptached as a result of handling an event. They represent the "business logic" you are looking to implement. 

Decorators provided for: 

* Validation (via Data Annotations) 
* Transactions 
* Resiliency (via Polly)
* Tracing / Diagnostics (via System.Diagnostics.DiagnosticSource)
* Logging (via Microsoft.Extensions.Logging)