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
        
        //campos que me faltaban
        public DateOnly? fechaNacimiento { get; set; }
public string? departamento { get; set; }
public string? municipio { get; set; }
public string? barrio { get; set; }
public string? direccion { get; set; }
public string? preguntaSecreta { get; set; }
public string? respuestaSecreta { get; set; }
public bool? autorizaCorreo { get; set; }    // ← nullable
public bool? autorizaCelular { get; set; }   // ← nullable

[Column("resetToken")]
public string? ResetToken { get; set; }

[Column("resetTokenExpiration")]
public DateTime? ResetTokenExpiration { get; set; }
    }
}