using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoundReserves.Models
{
    [Table("accommodation")]
    public class Accommodation
    {
        [Key]
        public int idAccommodation { get; set; }

        [ForeignKey("Sede")]
        public int idsede { get; set; }

        public string? name { get; set; }

        public string? number { get; set; }

        public int maximumPerson { get; set; }

        public string? description { get; set; }

        public string? state { get; set; }

        // Relación
        public Sede? Sede { get; set; }
    }
}