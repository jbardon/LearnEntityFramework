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
        private readonly SqliteConnection sqliteConnection;

        public InMemoryDb(ITestOutputHelper output) {
            this.output = output;

            this.sqliteConnection = new SqliteConnection("Filename=:memory:");
            this.sqliteConnection.Open();

            this.dbContext = this.CreateContext();
        }

        public T CreateContext() {
            DbContextOptions<T> options = new DbContextOptionsBuilder<T>()
                .UseSqlite(this.sqliteConnection)

                // https://docs.microsoft.com/en-us/ef/core/logging-events-diagnostics/simple-logging
                .LogTo(output.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .Options;

            var dbContext = Activator.CreateInstance(typeof(T), new object[] { options }) as T;
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            return dbContext;
        }
    }
}