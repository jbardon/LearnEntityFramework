using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace LearnEntityFramework.OptimizeQueries
{
    /*
      Resources:
        - https://docs.microsoft.com/en-us/ef/ef6/saving/change-tracking/property-values#setting-current-or-original-values-from-another-object
    */
    public class OptimizeQueriesTest : InMemoryDb<OptimizeQueriesContext>
    {
        public OptimizeQueriesTest(ITestOutputHelper output) : base(output)
        {}

        [Fact]
        public async Task SelectFewFields()
        {
            var entity = new MyEntity {
                Id = 1,
                Name = "Hello"
            };
            await dbContext.MyEntity.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.MyEntity
                // Projection: transform entity in another shape
                .Select(entity => new MyEntity 
                {
                    Id = entity.Id
                })
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Null(result.Name);
        }

        [Fact]
        public async Task SelectFewFieldsWithNestedEntity()
        {
            var parent = new Parent
            {
                Id = 1,
                Name = "Parent",
                Child = new Child
                {
                    Id = 11,
                    Name = "Child"
                }
            };
            await dbContext.Parent.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.Parent
                .Include(parent => parent.Child)

                // Projection means manual mapping for what should be returned
                .Select(parent => new Parent
                {
                    Id = parent.Id,
                    Child = new Child
                    {
                        Id = parent.Child.Id
                    }
                })
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.Null(result.Name);
            Assert.Equal(parent.Id, result.Id);

            Assert.NotNull(result.Child);
            Assert.Null(result.Child.Name);
            Assert.Equal(parent.Child.Id, result.Child.Id);
        }

        [Fact]
        public async Task UpdateFewFieldWithFind()
        {
            var entity = new MyEntity
            {
                Id = 1,
                Name = "Hello",
                StartDate = "05/08/2021",
                EndDate = "06/08/2021",
            };
            await dbContext.MyEntity.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            // Be carefull it does a select of all fields
            var updatedEntity = dbContext.MyEntity.Find(entity.Id);
            updatedEntity.Name = "Updated";

            var nameProperty = dbContext.Entry(updatedEntity).Property(entity => entity.Name);
            Assert.Equal(entity.Name, nameProperty.OriginalValue);
            Assert.Equal(updatedEntity.Name, nameProperty.CurrentValue);

            await dbContext.SaveChangesAsync();

            var result = await dbContext.MyEntity
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(updatedEntity.Name, result.Name);
            Assert.Equal(entity.StartDate, result.StartDate);
            Assert.Equal(entity.EndDate, result.EndDate);
        }

        [Fact]
        public async Task UpdateFewFieldWithEntryPropertyAndTracking()
        {
            var entity = new MyEntity
            {
                Id = 1,
                Name = "Hello",
                StartDate = "05/08/2021",
                EndDate = "06/08/2021",
            };
            await dbContext.MyEntity.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();
            
            // Update
            var updatedEntity = new MyEntity
            {
                Id = 1,
                Name = "Updated",
                StartDate = "NewDate"
            };

            // Mark Name as modified
            var nameProperty = dbContext.Entry(updatedEntity).Property(entity => entity.Name);
            nameProperty.IsModified = true;

            await dbContext.SaveChangesAsync();

            // Check if updated (with tracking enabled by default)
            var result = await dbContext.MyEntity
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(updatedEntity.Name, result.Name);

            // Values taken from context (updatedEntity)
            Assert.Equal(updatedEntity.StartDate, result.StartDate);
            Assert.Equal(updatedEntity.EndDate, result.EndDate);
        }

        [Fact]
        public async Task UpdateFewFieldWithEntryPropertyAndNoTracking()
        {
            var entity = new MyEntity
            {
                Id = 1,
                Name = "Hello",
                StartDate = "05/08/2021",
                EndDate = "06/08/2021",
            };
            await dbContext.MyEntity.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            // Update
            var updatedEntity = new MyEntity
            {
                Id = 1,
                Name = "Updated",
                StartDate = "NewDate"
            };
            dbContext.Entry(updatedEntity).Property(entity => entity.Name).IsModified = true;
            await dbContext.SaveChangesAsync();

            // Check if updated
            var result = await dbContext.MyEntity
                .AsNoTracking() // Disable tracking
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(updatedEntity.Name, result.Name);

            // Works fine
            // StartDate not updated because not marked as modified
            Assert.Equal(entity.StartDate, result.StartDate);
            Assert.Equal(entity.EndDate, result.EndDate);
        }

        [Fact]
        public async Task UpdateFewFieldWithEntryState()
        {
            var entity = new MyEntity
            {
                Id = 1,
                Name = "Hello",
                StartDate = "05/08/2021",
                EndDate = "06/08/2021",
            };
            await dbContext.MyEntity.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            // Update
            var updatedEntity = new MyEntity
            {
                Id = 1,
                Name = "Updated"
            };

            // Entry method to track changes and modify state
            dbContext.Entry(updatedEntity).State = EntityState.Unchanged;
            updatedEntity.Name = "Updated";
            await dbContext.SaveChangesAsync();

            // Check if updated (with traking enabled by default)
            var result = await dbContext.MyEntity
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(updatedEntity.Name, result.Name);

            // Values taken from context (updatedEntity)
            Assert.NotEqual(entity.StartDate, result.StartDate);
            Assert.NotEqual(entity.EndDate, result.EndDate);
        }

        [Fact]
        public async Task UpdateFewFieldWithAttach()
        {
            var entity = new MyEntity
            {
                Id = 1,
                Name = "Hello",
                StartDate = "05/08/2021",
                EndDate = "06/08/2021",
            };
            await dbContext.MyEntity.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            // Update
            var updatedEntity = new MyEntity
            {
                Id = 1
            };

            // Attach method to track changes
            dbContext.MyEntity.Attach(updatedEntity);
            updatedEntity.Name = "Updated";
            await dbContext.SaveChangesAsync();

            // Check if updated (with traking enabled by default)
            var result = await dbContext.MyEntity
                .AsNoTracking()
                .FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(updatedEntity.Name, result.Name);
            Assert.Equal(entity.StartDate, result.StartDate);
            Assert.Equal(entity.EndDate, result.EndDate);
        }

        [Fact]
        public async Task UpdateDangerouslyWithCompleteDto()
        {
            var entity = new MyEntity
            {
                Id = 1,
                Name = "Hello",
                StartDate = "06/08/2021"
            };
            await dbContext.MyEntity.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            var dto = new MyEntity
            {
                Id = 1,
                Name = "Bye",
            };
            var loadedEntity = await dbContext.MyEntity.FirstOrDefaultAsync();
            dbContext.Entry(loadedEntity).CurrentValues.SetValues(dto);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.MyEntity.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);

            // Properties without value in DTO are updated to null
            Assert.Null(result.StartDate);
        }

        [Fact]
        public async Task UpdateDangerouslyWithPartialDto()
        {
            var entity = new MyEntity
            {
                Id = 1,
                Name = "Hello",
                StartDate = "06/08/2021"
            };
            await dbContext.MyEntity.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            var dto = new
            {
                Id = 1,
                Name = "Bye"
            };
            var loadedEntity = await dbContext.MyEntity.FirstOrDefaultAsync();
            dbContext.Entry(loadedEntity).CurrentValues.SetValues(dto);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.MyEntity.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);

            // Not modified by not await property of dto
            Assert.Equal(entity.StartDate, result.StartDate);
        }
    }
}
