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
    }
}
