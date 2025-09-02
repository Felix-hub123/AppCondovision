using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome completo não pode exceder 100 caracteres.")]
        [Display(Name = "Nome Completo")]
        public string? FullName { get; set; }


        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email não é válido.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "A password é obrigatória.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string? Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Password")]
        [Compare("Password", ErrorMessage = "A password e a confirmação da password não correspondem.")]
        public string? ConfirmPassword { get; set; }


        [Required(ErrorMessage = "O contribuinte é obrigatório.")]
        [StringLength(20, ErrorMessage = "O contribuinte não pode exceder 20 caracteres.")]
        [Display(Name = "Contribuinte")]
        public string? TaxId { get; set; }


        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DateOfBirth { get; set; }


        [StringLength(200, ErrorMessage = "A morada não pode exceder 200 caracteres.")]
        [Display(Name = "Morada")]
        public string? Address { get; set; }


        [Required(ErrorMessage = "A empresa é obrigatória.")]
        [Display(Name = "Empresa")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "A role é obrigatória.")]
        [Display(Name = "Perfil")]
        public string? RoleName { get; set; }

        [Required(ErrorMessage = "O número de telefone é obrigatório.")]
        [StringLength(20, ErrorMessage = "O número de telefone não pode exceder 20 caracteres.")]
        [Phone(ErrorMessage = "O número de telefone não é válido.")]
        [Display(Name = "Número de Telefone")]
        public string? PhoneNumber { get; set; }


        [Display(Name = "Tipo de Utilizador")]
        public string? UserType { get; set; }
    }
}
