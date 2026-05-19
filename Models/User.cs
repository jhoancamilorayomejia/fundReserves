using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoundReserves.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int iduser { get; set; }

        public required string cedula { get; set; }

        public required string name { get; set; }

        public required string lastname { get; set; }
        public required string phone { get; set; }

        public required string email { get; set; }

        public required string password { get; set; }

        public required string rol { get; set; }

        public DateTime createdAt { get; set; } = DateTime.Now;
    }
}