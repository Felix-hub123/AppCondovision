using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password Atual")]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MinLength(6, ErrorMessage = "O campo {0} deve ter pelo menos {1} caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Password")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Compare("NewPassword", ErrorMessage = "A nova password e a confirmação não coincidem.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nova Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
