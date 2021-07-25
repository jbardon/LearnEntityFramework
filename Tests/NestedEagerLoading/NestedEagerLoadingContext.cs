using Microsoft.EntityFrameworkCore;

namespace LearnEntityFramework.NestedEagerLoading
{
    public class NestedEagerLoadingContext : DbContext
    {
        public DbSet<GrandParent> GrandParents { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Child1> Child1 { get; set; }
        public DbSet<Child2> Child2 { get; set; }
        public DbSet<Baby1> Baby1 { get; set; }

        public NestedEagerLoadingContext() : base() {}
        public NestedEagerLoadingContext(DbContextOptions<NestedEagerLoadingContext> options): base(options) {}
    }
}
