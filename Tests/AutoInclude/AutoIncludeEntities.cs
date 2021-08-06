using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LearnEntityFramework.AutoInclude
{
  // With AutoInclude
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

  // With OwnedType
  [Table("Parent2")]
  public class Parent2
  {
      [Key]
      public int Id { get; set; }
      public string Name { get; set; }
      public Child2 Child { get; set; }
  }

  [Table("Child2")]
  [Owned]
  public class Child2
  {
      [Key]
      public int Id { get; set; }
      public string Name { get; set; }
  }

  // With two nested levels (use method extensions)
  [Table("Parent3")]
  public class Parent3
  {
      [Key]
      public int Id { get; set; }
      public string Name { get; set; }
      public Child3 Child { get; set; }
  }

  [Table("Child3")]
  public class Child3
  {
      [Key]
      public int Id { get; set; }
      public string Name { get; set; }
      public Baby3 Baby { get; set; }
  }

  [Table("Baby3")]
  public class Baby3
  {
      [Key]
      public int Id { get; set; }
      public string Name { get; set; }
  }
}