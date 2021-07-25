using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace LearnEntityFramework
{
    public abstract class InMemoryDb<T> where T : DbContext, new()
    {
        protected readonly T dbContext;
        protected readonly ITestOutputHelper output;

        public InMemoryDb(ITestOutputHelper output) {
            this.output = output;

            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            DbContextOptions<T> options = new DbContextOptionsBuilder<T>()
                .UseSqlite(connection)

                // https://docs.microsoft.com/en-us/ef/core/logging-events-diagnostics/simple-logging
                .LogTo(output.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .Options;

            var dbContext = Activator.CreateInstance(typeof(T), new object[] { options }) as T;

            // https://docs.microsoft.com/en-us/ef/core/querying/tracking
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            this.dbContext = dbContext;
        }
    }
}