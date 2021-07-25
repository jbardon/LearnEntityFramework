using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace LearnEntityFramework.SimpleEntity
{
    public class SimpleEntityTest : InMemoryDb<MyContext>
    {
        public SimpleEntityTest(ITestOutputHelper output) : base(output)
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

    [Table("MyEntity")]
    public class MyEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MyContext : DbContext
    {
        public DbSet<MyEntity> MyEntity { get; set; }

        public MyContext() : base() {}
        public MyContext(DbContextOptions<MyContext> options): base(options) {}
    }
}
