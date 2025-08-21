using System.ComponentModel.DataAnnotations;

namespace CondoVision.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }
    }
}
