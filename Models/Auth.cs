using System.ComponentModel.DataAnnotations;

namespace FoundReserves.Models
{
    public class LoginModel
    {
        [Required]
        public required string Cedula { get; set; }

        [Required]
        public required string Password { get; set; }
    }

   public class RegisterModel
{
    [Required]
    public required string Cedula { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Lastname { get; set; }

    [Required]
    public required string Phone { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(6)]
    public required string Password { get; set; }

    // Ya no es [Required], tiene valor por defecto
    public string Rol { get; set; } = "Customer";


    // campos que me faltaban
    public DateOnly? FechaNacimiento { get; set; }
public string? Departamento { get; set; }
public string? Municipio { get; set; }
public string? Barrio { get; set; }
public string? Direccion { get; set; }
public string? PreguntaSecreta { get; set; }
public string? RespuestaSecreta { get; set; }
public bool? AutorizaCorreo { get; set; }
public bool? AutorizaCelular { get; set; }
}
}