using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEntityFramework.GettingStarted
{
    [Table("MyEntity")]
    public class MyEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
