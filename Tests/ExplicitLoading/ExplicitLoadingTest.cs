using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

// https://docs.microsoft.com/en-us/ef/core/querying/related-data/explicit
// https://www.entityframeworktutorial.net/EntityFramework4.3/explicit-loading-with-dbcontext.aspx
namespace LearnEntityFramework.ExplicitLoading
{
    public class ExplicitLoadingTest : InMemoryDb<ExplicitLoadingContext>
    {
        public ExplicitLoadingTest(ITestOutputHelper output) : base(output)
        {}

        [Fact]
        public async Task LoadAllChildren()
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
        public async Task LoadChildrenWithFilter()
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
        public async Task WithoutExplicitLoadingAndLinqMethodSyntax()
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
        public async Task WithoutExplicitLoadingAndLinqQuerySyntax()
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
}
