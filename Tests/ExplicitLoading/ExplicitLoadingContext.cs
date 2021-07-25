using Microsoft.EntityFrameworkCore;

namespace LearnEntityFramework.ExplicitLoading
{
    public class ExplicitLoadingContext : DbContext
    {
        public DbSet<Parent> Parent { get; set; }
        public DbSet<Child> Child { get; set; }

        public ExplicitLoadingContext() : base() {}
        public ExplicitLoadingContext(DbContextOptions<ExplicitLoadingContext> options): base(options) {}
    }
}
