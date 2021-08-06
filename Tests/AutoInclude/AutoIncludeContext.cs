using Microsoft.EntityFrameworkCore;

namespace LearnEntityFramework.AutoInclude
{
  public class AutoIncludeContext : DbContext
  {
    // With Auto include
    public DbSet<Parent> Parent { get; set; }
    public DbSet<Child> Child { get; set; }

    // With Owned  attribute
    public DbSet<Parent2> Parent2 { get; set; }
    public DbSet<Child2> Child2 { get; set; }

    public DbSet<Parent3> Parent3 { get; set; }
    public DbSet<Child3> Child3 { get; set; }
    public DbSet<Baby3> Baby3 { get; set; }

    public AutoIncludeContext() : base() { }
    public AutoIncludeContext(DbContextOptions<AutoIncludeContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // https://dotnetcoretutorials.com/2021/03/07/eager-load-navigation-properties-by-default-in-ef-core/
      modelBuilder.Entity<Parent>()
          .Navigation(parent => parent.Child)
          .AutoInclude();
    }
  }
}