using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEntityFramework.AutoInclude
{
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