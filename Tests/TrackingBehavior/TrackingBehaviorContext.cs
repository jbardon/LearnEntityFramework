using Microsoft.EntityFrameworkCore;

namespace LearnEntityFramework.TrackingBehavior
{
    public class TrackingBehaviorContext : DbContext
    {
        public DbSet<Parent> Parent { get; set; }
        public DbSet<Child> Child { get; set; }

        public TrackingBehaviorContext() : base() {}
        public TrackingBehaviorContext(DbContextOptions<TrackingBehaviorContext> options): base(options) {}
    }
}
