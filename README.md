# LearnEntityFramework

Xunit test project to experiment with Entity Framework (EF) Core 5.  
Requirements: .NET 5 SDK

## Important notes

Tests runs against In-Memory SQLite database. It may not fit the production database and has limitations. It means unit tests [may succeed  when production code fails](https://docs.microsoft.com/en-us/ef/core/testing/testing-sample#issues-using-different-database-providers) and vice versa.  


[Tracking is sometimes disabled](https://docs.microsoft.com/en-us/ef/core/querying/tracking) to avoid false positive tests. The same context inserts data before the test and execute queries which allows EF to keep track of inserted data. It means a query without `Include` is able to return child entities when tracking is enabled.

<details>
<summary>Several ways to disable tracking</summary>

```cs
// Disable for all queries (before inserts)
dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

// Disable on single query
dbContext.MyEntity.AsNoTracking().FirstOrDefaultAsync();

// Clear after inserts
dbContext.ChangeTracker.Clear();
```

</details>

## TODO
* Getting started: More exemple without get (update, delete with attach)
    - https://stackoverflow.com/questions/58367172/ef-core-updating-an-entity-without-querying-it-first
    - https://stackoverflow.com/questions/4218566/update-a-record-without-first-querying/8566906
* AutoInclude
    - https://stackoverflow.com/questions/48462746/how-to-include-only-selected-properties-on-related-entities
    - https://stackoverflow.com/questions/14512285/entity-framework-is-there-a-way-to-automatically-eager-load-child-entities-wit
    - https://stackoverflow.com/questions/10822656/entity-framework-include-multiple-levels-of-properties
* Others
    - https://davecallan.com/asnotracking-identity-resolution-available-entity-framework-core-5-0/
    - https://www.thereformedprogrammer.net/ef-core-in-depth-what-happens-when-ef-core-reads-from-the-database/
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