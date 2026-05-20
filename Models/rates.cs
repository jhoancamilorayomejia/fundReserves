using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoundReserves.Models
{
    [Table("rates")]
    public class Rate
    {
        [Key]
        public int idPrice { get; set; }

        [Required]
        public int idAccommodation { get; set; }

        [Required]
        public int idSeason { get; set; }

        [Required]
        public int minimumPerson { get; set; }

        [Required]
        public int maximumPerson { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal priceNight { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal pricePersonAdditional { get; set; }
    }
}