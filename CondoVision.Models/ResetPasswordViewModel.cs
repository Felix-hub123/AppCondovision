using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Password")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "A nova password e a confirmação não coincidem.")]
        [Display(Name = "Confirmar Nova Password")]
        public string? ConfirmPassword { get; set; }

        public string? UserId { get; set; }
        public string? Token { get; set; }
        public string? Email { get; set; }
    }
}
