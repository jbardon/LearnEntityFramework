using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace LearnEntityFramework.TrackingBehavior
{
    // https://docs.microsoft.com/en-us/ef/core/change-tracking/
    // https://docs.microsoft.com/en-us/ef/core/querying/tracking
    public class TrackingBehaviorTest : InMemoryDb<TrackingBehaviorContext>
    {
        public TrackingBehaviorTest(ITestOutputHelper output) : base(output)
        {}

        [Fact]
        public async Task WithTracking()
        {
            var parent = new Parent
            {
                Id = 1,
                Name = "Parent",
                Children = new[] {
                    new Child {
                        Id = 11,
                        Name = "Child1"
                    }
                }
            };
            await dbContext.Parent.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            // Only select parent but Children reference
            // filled with context (where child was added to before)
            //
            // No need for Include in the request
            var result = await dbContext.Parent.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.NotNull(result.Children);
        }

        [Fact]
        public async Task ClearTracking()
        {
            var parent = new Parent
            {
                Id = 1,
                Name = "Parent",
                Children = new[] {
                    new Child {
                        Id = 11,
                        Name = "Child1"
                    }
                }
            };
            await dbContext.Parent.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            // Context is empty
            // EF won't be able to fill children reference
            // with entities in the context
            dbContext.ChangeTracker.Clear();

            var result = await dbContext.Parent.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Null(result.Children);
        }

        [Fact]
        public async Task DisableContextTracking()
        {
            var parent = new Parent
            {
                Id = 1,
                Name = "Parent",
                Children = new[] {
                    new Child {
                        Id = 11,
                        Name = "Child1"
                    }
                }
            };

            // Disable tracking for this context
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            await dbContext.Parent.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.Parent.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Null(result.Children);
        }

        [Fact]
        public async Task DisableOnQuery()
        {
            var parent = new Parent
            {
                Id = 1,
                Name = "Parent",
                Children = new[] {
                    new Child {
                        Id = 11,
                        Name = "Child1"
                    }
                }
            };

            await dbContext.Parent.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            // Disable for this query
            // - no Children reference filled
            // - no Update traking
            var untrackedParent = await dbContext.Parent
                .AsNoTracking() 
                .FirstOrDefaultAsync();

            // No Children reference filled
            Assert.NotNull(untrackedParent);
            Assert.Null(untrackedParent.Children);

            // No Update tracking
            untrackedParent.Name = "Bye";
            await dbContext.SaveChangesAsync();

            var result = await dbContext.Parent.FirstOrDefaultAsync();
            Assert.Equal("Parent", result.Name);
        }

        [Fact(Skip = "Don't understand well identity resolution")]
        public async Task DisableOnQueryAndKeepIdentityResolution()
        {
            var parent = new Parent
            {
                Id = 1,
                Name = "Parent",
                Children = new[] {
                    new Child {
                        Id = 11,
                        Name = "Child1"
                    }
                }
            };

            await dbContext.Parent.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            // Disable for this query
            // but keep identity resolution
            var untrackedParent = await dbContext.Parent
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefaultAsync();

            // Children reference filled
            // thanks to identity resolution
            Assert.NotNull(untrackedParent);
            Assert.NotNull(untrackedParent.Children);

            // No Update tracking
            untrackedParent.Name = "Bye";
            await dbContext.SaveChangesAsync();

            var result = await dbContext.Parent.FirstOrDefaultAsync();
            Assert.Equal("Parent", result.Name);
        }

        [Fact]
        public async Task WithNewContext()
        {
            var parent = new Parent
            {
                Id = 1,
                Name = "Parent",
                Children = new[] {
                    new Child {
                        Id = 11,
                        Name = "Child1"
                    }
                }
            };

            await dbContext.Parent.AddAsync(parent);
            await dbContext.SaveChangesAsync();

            // Tracking for this context don't know about added child
            // Have using keyword to match often used code on the internet
            using (var otherContext = this.CreateContext()) {
                var result = await otherContext.Parent.FirstOrDefaultAsync();
                Assert.NotNull(result);
                Assert.Null(result.Children);
            }
        }
    }
}
