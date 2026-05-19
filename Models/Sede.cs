using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoundReserves.Models
{
    [Table("sedes")]
    public class Sede
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idSede { get; set; }

        public required string name { get; set; }
        public required string city { get; set; }
        public required string region { get; set; }
        public string? description { get; set; }
        public int maximumCapacity { get; set; }
        public required string type { get; set; }
        public decimal priceLaundry { get; set; }
    }
}