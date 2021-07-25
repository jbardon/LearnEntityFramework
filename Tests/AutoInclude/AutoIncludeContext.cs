using Microsoft.EntityFrameworkCore;

namespace LearnEntityFramework.AutoInclude
{
  public class AutoIncludeContext : DbContext
  {
    public DbSet<Parent> Parent { get; set; }
    public DbSet<Child> Child { get; set; }

    public AutoIncludeContext() : base() { }
    public AutoIncludeContext(DbContextOptions<AutoIncludeContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // https://dotnetcoretutorials.com/2021/03/07/eager-load-navigation-properties-by-default-in-ef-core/
      modelBuilder.Entity<Parent>()
          .Navigation(x => x.Child)
          .AutoInclude();
    }
  }
}