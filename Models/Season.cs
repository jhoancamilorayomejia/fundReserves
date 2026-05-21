using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoundReserves.Models
{
    [Table("seasons")]
    public class Season
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idSeason { get; set; }
        public string name { get; set; } = string.Empty;

        public required string type { get; set; }
    }
}