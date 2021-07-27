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
        public async Task InsertAndSelect()
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

        [Fact]
        public async Task Update()
        {
            var entity = new MyEntity
            {
                Id = 1,
                Name = "Hello"
            };
            await dbContext.MyEntity.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            var loadedEntity = await dbContext.MyEntity.FirstOrDefaultAsync();
            loadedEntity.Name = "Bye";
            await dbContext.SaveChangesAsync();

            var result = await dbContext.MyEntity.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal("Bye", result.Name);
        }

        [Fact]
        public async Task Delete()
        {
            var entity = new MyEntity
            {
                Id = 1,
                Name = "Hello"
            };
            await dbContext.MyEntity.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            var loadedEntity = await dbContext.MyEntity.FirstOrDefaultAsync();
            dbContext.Remove(loadedEntity);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.MyEntity.CountAsync();
            Assert.Equal(0, result);
        }
    }
}
