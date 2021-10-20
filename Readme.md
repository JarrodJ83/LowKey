# LowKey
LowKey aims to be an unobtrusive, highly extensible set of building blocks for building dotnet APIs and Services. This includes abstractions for interacting with data stores, sending and recieving events, and implementing basic CQRS.

## Key Design Points
* Core packages containing abstractions will have no dependencies
* SOLID principles will be followed to allow for a highly maintainable and extensible set of libraries
* Avoid making consumers "sprinkle" custom attributes and interfaces throughout their own codebase in order to leverage LowKey (crucial to lightweight and unobtrusive design). A consumer's business code should only need to rely on the core abstractions. (Thanks to [Jeremy D. Miller](https://jeremydmiller.com/) and his [JasperFx](https://jasperfx.github.io) for inspiration here). The host startup code will configure the implementations and features of LowKey.
* Avoid reflection aside from in service registrations