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
private readonly Database _database;

public async Task<Product?> Handle(ProductById query, CancellationToken) =>
     _dataStore.QueryAsync(_database, conn =>        
        conn.QuerySingleOrDefaultAsync<Product>("select id, name from product where id = @id", query)
    );
```

### Using a EFCore

```
private readonly IDataStore<ProductDbContext> _dataStore;
private readonly Database _database;

public async Task<Product?> Handle(ProductById query, CancellationToken) =>
     _dataStore.QueryAsync(_database, ctx =>        
        ctx.Products.SingleOrDefault(product => product.Id == query.Id);
    );
```