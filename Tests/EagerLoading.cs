using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

// https://docs.microsoft.com/en-us/ef/core/querying/related-data/explicit
// https://www.entityframeworktutorial.net/EntityFramework4.3/explicit-loading-with-dbcontext.aspx
namespace LearnEntityFramework.EagerLoading
{
    public class EagerLoadingTest : InMemoryDb<MyContext>
    {
        public EagerLoadingTest(ITestOutputHelper output) : base(output)
        {}

        [Fact]
        public async Task Test1()
        {
            var parent = new Parent
            {
                Id = 1,
                Name = "Parent",
                Children = new[] {
                    new Child {
                        Id = 11,
                        Name = "Child1"
                    },
                    new Child {
                        Id = 12,
                        Name = "Child2"
                    }
                }
            };
            await dbContext.Parent.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            var result = dbContext.Parent
                .AsTracking() // Required when using dbContext.Entry
                .FirstOrDefault();

            // Explicit loading
            await dbContext.Entry(result)
                .Collection(parent => parent.Children)
                .LoadAsync();

            Assert.Equal(2, result.Children.Count);
            Assert.Equal("Child1", result.Children.ToArray()[0].Name);
        }
        [Fact]
        public async Task Test2()
        {
            var parent = new Parent
            {
                Id = 1,
                Name = "Parent",
                Children = new[] {
                    new Child {
                        Id = 11,
                        Name = "Child1"
                    },
                    new Child {
                        Id = 12,
                        Name = "Child2"
                    }
                }
            };
            await dbContext.Parent.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            var parentResult = dbContext.Parent
                .AsTracking()
                .FirstOrDefault();

            var result = await dbContext.Entry(parentResult)
                .Collection(parent => parent.Children)
                .Query()
                .Where(child => child.Id == 11)
                .ToListAsync();

            Assert.Single(result);
            Assert.Equal("Child1", result[0].Name);
        }

        [Fact]
        public async Task Test3()
        {
            var parent = new Parent
            {
                Id = 1,
                Name = "Parent",
                Children = new[] {
                    new Child {
                        Id = 11,
                        Name = "Child1"
                    },
                    new Child {
                        Id = 12,
                        Name = "Child2"
                    }
                }
            };
            await dbContext.Parent.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            var result = dbContext.Parent
                .Where(parent => parent.Children.Any(child => child.Id == 11))
                .FirstOrDefault();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Test4()
        {
            var dbParent = new Parent
            {
                Id = 1,
                Name = "Parent",
                Children = new[] {
                    new Child {
                        Id = 11,
                        Name = "Child1"
                    },
                    new Child {
                        Id = 12,
                        Name = "Child2"
                    }
                }
            };
            await dbContext.Parent.AddAsync(dbParent);
            await dbContext.SaveChangesAsync();

            var result = await (from parent in dbContext.Parent
                        join child in dbContext.Child on parent.Id equals child.ParentId
                        where child.Id == 11
                        select parent).FirstOrDefaultAsync();

            Assert.NotNull(result);
        }
    }

    [Table("Parent")]
    public class Parent
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("ParentId")]
        public ICollection<Child> Children { get; set; }
    }

    [Table("Child")]
    public class Child
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
  }

    public class MyContext : DbContext
    {
        public DbSet<Parent> Parent { get; set; }
        public DbSet<Child> Child { get; set; }

        public MyContext() : base() {}
        public MyContext(DbContextOptions<MyContext> options): base(options) {}
    }
}
