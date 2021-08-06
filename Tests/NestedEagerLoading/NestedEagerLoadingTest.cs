using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace LearnEntityFramework.NestedEagerLoading
{
    public class NestedEagerLoadingTest : InMemoryDb<NestedEagerLoadingContext>
    {
        public NestedEagerLoadingTest(ITestOutputHelper output) : base(output)
        {
            // Ensure entities added before the test aren't in memory
            // It makes sure the tests won't work without Include
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        [Fact]
        public async Task ShortSyntaxAndSeparateAdd()
        {
            var grandParent = new GrandParent
            {
                Id = 1,
                Name = "Grand Parent",
            };
            await dbContext.GrandParent.AddAsync(grandParent);

            var parent = new Parent
            {
                Id = 11,
                Name = "Parent",
                GrandParentId = 1
            };
            await dbContext.Parent.AddAsync(parent);

            var child1 = new Child1
            {
                Id = 111,
                Name = "Child1",
                ParentId = 11
            };
            await dbContext.Child1.AddAsync(child1);

            var child2 = new Child2
            {
                Id = 112,
                Name = "Child2",
                ParentId = 11
            };
            await dbContext.Child2.AddAsync(child2);

            var baby = new Baby
            {
                Id = 1121,
                Name = "Baby",
                Child2Id = 112
            };
            await dbContext.Baby.AddAsync(baby);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.GrandParent
                .Include(grandParent => grandParent.Parent.Child1)
                .Include(grandParent => grandParent.Parent.Child2)
                .ThenInclude(child2 => child2.Baby)
                .FirstOrDefaultAsync();

            Assert.Equal(grandParent.Id, result.Id);
            Assert.Equal(grandParent.Parent.Id, result.Parent.Id);
            Assert.Equal(grandParent.Parent.Child1.ToArray()[0].Id, result.Parent.Child1.ToArray()[0].Id);
            Assert.Equal(grandParent.Parent.Child2.ToArray()[0].Id, result.Parent.Child2.ToArray()[0].Id);
            Assert.Equal(grandParent.Parent.Child2.ToArray()[0].Baby.Id, result.Parent.Child2.ToArray()[0].Baby.Id);
        }

        [Fact]
        public async Task ShortSyntaxAndSingleAdd()
        {
            var grandParent = new GrandParent {
                Id = 1,
                Name = "Grand Parent",
                Parent = new Parent {
                    Id = 11,
                    Name = "Parent",
                    Child1 = new [] {
                        new Child1 {
                            Id = 111,
                            Name = "Child1"
                        }
                    },
                    Child2 = new[] {
                        new Child2 {
                            Id = 112,
                            Name = "Child2",
                            Baby = new Baby {
                                Id = 1121,
                                Name = "Baby"
                            }
                        }
                    }
                }
            };
            await dbContext.GrandParent.AddAsync(grandParent);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.GrandParent
                .Include(grandParent => grandParent.Parent.Child1)
                .Include(grandParent => grandParent.Parent.Child2)
                .ThenInclude(child2 => child2.Baby)
                .FirstOrDefaultAsync();

            Assert.Equal(grandParent.Id, result.Id);
            Assert.Equal(grandParent.Parent.Id, result.Parent.Id);
            Assert.Equal(grandParent.Parent.Child1.ToArray()[0].Id, result.Parent.Child1.ToArray()[0].Id);
            Assert.Equal(grandParent.Parent.Child2.ToArray()[0].Id, result.Parent.Child2.ToArray()[0].Id);
            Assert.Equal(grandParent.Parent.Child2.ToArray()[0].Baby.Id, result.Parent.Child2.ToArray()[0].Baby.Id);
        }

        [Fact]
        public async Task LongerSyntaxSometimesNecessary()
        {
            var grandParent = new GrandParent {
                Id = 1,
                Name = "Grand Parent",
                Parent = new Parent {
                    Id = 11,
                    Name = "Parent",
                    Child1 = new [] {
                        new Child1 {
                            Id = 111,
                            Name = "Child1"
                        }
                    },
                    Child2 = new[] {
                        new Child2 {
                            Id = 112,
                            Name = "Child2",
                            Baby = new Baby {
                                Id = 1121,
                                Name = "Baby"
                            }
                        }
                    }
                }
            };
            await dbContext.GrandParent.AddAsync(grandParent);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.GrandParent
                .Include(grandParent => grandParent.Parent)

                // Not possible because Child2 is a list
                // .ThenInclude(parent => parent.Child2.Baby1)
                .ThenInclude(parent => parent.Child2)
                .ThenInclude(child2 => child2.Baby)

                // Can repeat same include multiple times
                // To use ThenInclude for other properties
                .Include(grandParent => grandParent.Parent)
                .ThenInclude(parent => parent.Child1)
                .FirstOrDefaultAsync();

            Assert.Equal(grandParent.Id, result.Id);
            Assert.Equal(grandParent.Parent.Id, result.Parent.Id);
            Assert.Equal(grandParent.Parent.Child1.ToArray()[0].Id, result.Parent.Child1.ToArray()[0].Id);
            Assert.Equal(grandParent.Parent.Child2.ToArray()[0].Id, result.Parent.Child2.ToArray()[0].Id);
            Assert.Equal(grandParent.Parent.Child2.ToArray()[0].Baby.Id, result.Parent.Child2.ToArray()[0].Baby.Id);
        }
    }
}
