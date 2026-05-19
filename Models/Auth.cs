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
}
}