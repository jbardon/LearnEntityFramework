using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEntityFramework.NestedEagerLoading
{
    [Table("GrandParent")]
    public class GrandParent
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Parent Parent { get; set; }
    }

    [Table("Parent")]
    public class Parent {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("ParentId")]
        public ICollection<Child1> Child1 { get; set; }

        [ForeignKey("ParentId")]
        public ICollection<Child2> Child2 { get; set; }

        [ForeignKey("GrandParent")]
        public int GrandParentId { get; set; }
  }

    [Table("Child1")]
    public class Child1
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
    }

    [Table("Child2")]
    public class Child2
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Baby1 Baby1 { get; set; }
        public int ParentId { get; set; }
    }

    [Table("Baby1")]
    public class Baby1
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("Child2")]
        public int Child2Id { get; set; }
    }
}
