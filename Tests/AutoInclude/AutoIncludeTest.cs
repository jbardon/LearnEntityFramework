using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace LearnEntityFramework.AutoInclude
{
    public class AutoIncludeTest : InMemoryDb<AutoIncludeContext>
    {
        public AutoIncludeTest(ITestOutputHelper output) : base(output)
        {}

        [Fact]
        public async Task WithAutoInclude()
        {
            var parent = new Parent
            {
                Id = 1,
                Name = "Parent",
                Child = new Child {
                    Id = 11,
                    Name = "Child1"
                }
            };
            await dbContext.Parent.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            // No need to Include Children
            // Enabled by default in AutoIncludeContext
            var result = await dbContext.Parent.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.NotNull(result.Child);
            Assert.Equal("Child1", result.Child.Name);
        }

        [Fact]
        public async Task WithoutAutoInclude()
        {
            var parent = new Parent
            {
                Id = 1,
                Name = "Parent",
                Child = new Child {
                    Id = 11,
                    Name = "Child1"
                }
            };
            await dbContext.Parent.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.Parent
                .IgnoreAutoIncludes() // Disable default auto include
                .AsNoTracking()       // Don't use entities in memory added above
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.Null(result.Child);
        }

        [Fact]
        public async Task WithOwnedType()
        {
            // Reference: https://stackoverflow.com/a/48479833
            var parent = new Parent2
            {
                Id = 1,
                Name = "Parent",
                Child = new Child2
                {
                    Id = 11,
                    Name = "Child1"
                }
            };
            await dbContext.Parent2.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.Parent2
                .AsNoTracking()       // Don't use entities in memory added above
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.NotNull(result.Child);
            Assert.Equal("Child1", result.Child.Name);
        }

        [Fact]
        public async Task WithExtensionMethods()
        {
            // Reference: https://stackoverflow.com/a/14520939
            var parent = new Parent3
            {
                Id = 1,
                Name = "Parent",
                Child = new Child3
                {
                    Id = 11,
                    Name = "Child1",
                    Baby = new Baby3
                    {
                        Id = 111,
                        Name = "Baby1"
                    }
                }
            };
            await dbContext.Parent3.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.Parent3
                .AsNoTracking()
                .Build() // Custom extension method
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.NotNull(result.Child);
            Assert.Equal("Child1", result.Child.Name);
        }
    }

    static class Parent3Extensions {
        public static IQueryable<Parent3> Build(this IQueryable<Parent3> query) {
            return query
                // .Include(parent => parent.Child) // Not needed because done in BuildChild
                .BuildChild();
        }

        public static IQueryable<Parent3> BuildChild(this IQueryable<Parent3> query)
        {
            // For collections:
            //  - query.Include(parent => parent.Children.Select(child => child.Baby))
            //  - query.Include(parent.Children).ThenInclude(child => child.Baby)
            return query.Include(parent => parent.Child.Baby);
        }
    }
}
