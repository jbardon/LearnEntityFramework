using Microsoft.EntityFrameworkCore;

namespace LearnEntityFramework.OptimizeQueries
{
    public class OptimizeQueriesContext : DbContext
    {
        public DbSet<MyEntity> MyEntity { get; set; }
        public DbSet<Parent> Parent { get; set; }
        public DbSet<Child> Child { get; set; }

        public OptimizeQueriesContext() : base() {}
        public OptimizeQueriesContext(DbContextOptions<OptimizeQueriesContext> options): base(options) {}
    }
}
