# LearnEntityFramework

Xunit test project to experiment with Entity Framework (EF) Core 5.  
Requirements: .NET 5 SDK

## Important notes

Tests runs against In-Memory SQLite database. It may not fit the production database and has limitations. It means unit tests [may succeed  when production code fails](https://docs.microsoft.com/en-us/ef/core/testing/testing-sample#issues-using-different-database-providers) and vice versa.  

[Tracking is disabled by default](https://docs.microsoft.com/en-us/ef/core/querying/tracking) to avoid false positive tests. The same context inserts data before the test and execute requests which allows EF to keep track of inserted data. It means a query without `Include` is able to return child entities when tracking is enabled.

## Run tests

```sh
# Using docker (not tested)
# docker run --rm --volume $(pwd):/project --workdir /project mcr.microsoft.com/dotnet/sdk:5.0 dotnet test

# All tests
dotnet test

# Include EF queries logging
dotnet test --logger "console;verbosity=detailed"

# Filter tests
dotnet test --filter "FullyQualifiedName~MultipleLevelEntityTest.Test2"
```