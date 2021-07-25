using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

/*
https://stackoverflow.com/questions/48462746/how-to-include-only-selected-properties-on-related-entities
https://stackoverflow.com/questions/14512285/entity-framework-is-there-a-way-to-automatically-eager-load-child-entities-wit
https://stackoverflow.com/questions/10822656/entity-framework-include-multiple-levels-of-properties
*/

namespace LearnEntityFramework.NestedEagerLoading
{
    public class NestedEagerLoadingTest : InMemoryDb<NestedEagerLoadingContext>
    {
        public NestedEagerLoadingTest(ITestOutputHelper output) : base(output)
        {}

        [Fact]
        public async Task ShortSyntaxAndSeparateAdd()
        {
            var grandParent = new GrandParent
            {
                Id = 1,
                Name = "Grand Parent",
            };
            await dbContext.GrandParents.AddAsync(grandParent);

            var parent = new Parent
            {
                Id = 11,
                Name = "Parent",
                GrandParentId = 1
            };
            await dbContext.Parents.AddAsync(parent);

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

            var baby1 = new Baby1
            {
                Id = 1121,
                Name = "Baby1",
                Child2Id = 112
            };
            await dbContext.Baby1.AddAsync(baby1);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.GrandParents
                .Include(grandParent => grandParent.Parent.Child1)
                .Include(grandParent => grandParent.Parent.Child2)
                .ThenInclude(child2 => child2.Baby1)
                .FirstOrDefaultAsync();

            Assert.Equal(grandParent.Id, result.Id);
            Assert.Equal(grandParent.Parent.Id, result.Parent.Id);
            Assert.Equal(grandParent.Parent.Child1.ToArray()[0].Id, result.Parent.Child1.ToArray()[0].Id);
            Assert.Equal(grandParent.Parent.Child2.ToArray()[0].Id, result.Parent.Child2.ToArray()[0].Id);
            Assert.Equal(grandParent.Parent.Child2.ToArray()[0].Baby1.Id, result.Parent.Child2.ToArray()[0].Baby1.Id);
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
                            Baby1 = new Baby1 {
                                Id = 1121,
                                Name = "Baby1"
                            }
                        }
                    }
                }
            };
            await dbContext.GrandParents.AddAsync(grandParent);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.GrandParents
                .Include(grandParent => grandParent.Parent.Child1)
                .Include(grandParent => grandParent.Parent.Child2)
                .ThenInclude(child2 => child2.Baby1)
                .FirstOrDefaultAsync();

            Assert.Equal(grandParent.Id, result.Id);
            Assert.Equal(grandParent.Parent.Id, result.Parent.Id);
            Assert.Equal(grandParent.Parent.Child1.ToArray()[0].Id, result.Parent.Child1.ToArray()[0].Id);
            Assert.Equal(grandParent.Parent.Child2.ToArray()[0].Id, result.Parent.Child2.ToArray()[0].Id);
            Assert.Equal(grandParent.Parent.Child2.ToArray()[0].Baby1.Id, result.Parent.Child2.ToArray()[0].Baby1.Id);
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
                            Baby1 = new Baby1 {
                                Id = 1121,
                                Name = "Baby1"
                            }
                        }
                    }
                }
            };
            await dbContext.GrandParents.AddAsync(grandParent);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.GrandParents
                .Include(grandParent => grandParent.Parent)
                .ThenInclude(parent => parent.Child1)
                .Include(grandParent => grandParent.Parent)
                .ThenInclude(parent => parent.Child2)

                // Not possible because Child2 is a list
                // .ThenInclude(parent => parent.Child2.Baby1)
                .ThenInclude(child2 => child2.Baby1)
                .FirstOrDefaultAsync();

            Assert.Equal(grandParent.Id, result.Id);
            Assert.Equal(grandParent.Parent.Id, result.Parent.Id);
            Assert.Equal(grandParent.Parent.Child1.ToArray()[0].Id, result.Parent.Child1.ToArray()[0].Id);
            Assert.Equal(grandParent.Parent.Child2.ToArray()[0].Id, result.Parent.Child2.ToArray()[0].Id);
            Assert.Equal(grandParent.Parent.Child2.ToArray()[0].Baby1.Id, result.Parent.Child2.ToArray()[0].Baby1.Id);
        }
    }
}
