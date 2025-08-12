using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class RegisterNewUserViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Nome Completo")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo {0} não é um endereço de e-mail válido.")]
        [Display(Name = "E-mail")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MinLength(6, ErrorMessage = "O campo {0} deve ter pelo menos {1} caracteres.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Compare("Password", ErrorMessage = "A password e a confirmação não coincidem.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Password")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Número de Telefone")]
        public string? PhoneNumber { get; set; }

    }
}
