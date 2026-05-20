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

        public DateTime dateStart { get; set; }

        public DateTime dateFinish { get; set; }

        public required string type { get; set; }
    }
}