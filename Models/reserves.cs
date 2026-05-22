using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoundReserves.Models
{
    [Table("reserves")]
    public class Reserve
    {
        [Key]
        [Column("idReserve")]
        public int IdReserve { get; set; }

        [Column("idUser")]
        public int IdUser { get; set; }

        [Column("idAccommodation")]
        public int IdAccommodation { get; set; }

        [Column("dateStart")]
        public DateTime DateStart { get; set; }

        [Column("dateFinish")]
        public DateTime DateFinish { get; set; }

        [Column("numberPerson")]
        public int NumberPerson { get; set; }

        [Column("totalCal", TypeName = "decimal(18,2)")]
        public decimal TotalCal { get; set; }

        [Column("state")]
        public string? State { get; set; }

        [Column("dateCreation")]
        public DateTime DateCreation { get; set; }

        [Column("serviceLaundry")]
        public bool ServiceLaundry { get; set; }
    }
}