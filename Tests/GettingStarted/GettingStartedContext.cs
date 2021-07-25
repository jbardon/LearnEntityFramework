using Microsoft.EntityFrameworkCore;

namespace LearnEntityFramework.GettingStarted
{
    public class GettingStartedContext : DbContext
    {
        public DbSet<MyEntity> MyEntity { get; set; }

        public GettingStartedContext() : base() {}
        public GettingStartedContext(DbContextOptions<GettingStartedContext> options): base(options) {}
    }
}
