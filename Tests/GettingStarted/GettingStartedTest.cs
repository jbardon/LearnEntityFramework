using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace LearnEntityFramework.GettingStarted
{
    public class GettingStartedTest : InMemoryDb<GettingStartedContext>
    {
        public GettingStartedTest(ITestOutputHelper output) : base(output)
        {}

        [Fact]
        public async Task Test1()
        {
            var entity = new MyEntity {
                Id = 1,
                Name = "Hello"
            };
            await dbContext.MyEntity.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.MyEntity.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
        }
    }
}
