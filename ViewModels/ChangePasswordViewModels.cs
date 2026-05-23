using System.ComponentModel.DataAnnotations;

namespace FoundReserves.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(40, MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirma tu contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}