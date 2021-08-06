using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEntityFramework.OptimizeQueries
{
    [Table("MyEntity")]
    public class MyEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    [Table("Parent")]
    public class Parent
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("ParentId")]
        public Child Child { get; set; }
    }

    [Table("Child")]
    public class Child
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
