using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEntityFramework.ExplicitLoading
{
    [Table("Parent")]
    public class Parent
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("ParentId")]
        public ICollection<Child> Children { get; set; }
    }

    [Table("Child")]
    public class Child
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
  }
}
