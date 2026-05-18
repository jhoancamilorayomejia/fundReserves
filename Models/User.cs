using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoundReserves.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public required string email { get; set; }

        public required string password { get; set; }

        public required string rol { get; set; }

        public DateTime createdAt { get; set; } = DateTime.UtcNow;
    }
}