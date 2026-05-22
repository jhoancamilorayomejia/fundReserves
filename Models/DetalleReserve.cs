using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoundReserves.Models
{
    public class DetalleReserve
    {
        [Key]
        public int IdDetalleReserve { get; set; }

        [Required]
        public int IdUser { get; set; }

        [Required]
        public int IdSede { get; set; }

        [Required]
        public DateTime DateStart { get; set; }

        [Required]
        public DateTime DateFinish { get; set; }

        [Required]
        public int NumberPerson { get; set; }

        [Required]
        public int NumberRooms { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal TotalCal { get; set; }

        [MaxLength(255)]
        public string? PaymentProof { get; set; }

        
    }
}