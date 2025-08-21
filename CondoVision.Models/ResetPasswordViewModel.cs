using System.ComponentModel.DataAnnotations;

namespace CondoVision.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string? UserId { get; set; }

        [Required]
        [Display(Name = "Novo Password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "O password deve ter pelo menos 6 caracteres.")]
        public string? NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirmar Password")]
        [Compare("NewPassword", ErrorMessage = "Os passwords não coincidem.")]
        public string? ConfirmPassword { get; set; }

        [Required]
        public string? Token { get; set; }

        [Display(Name = "Número de Telefone")]
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
