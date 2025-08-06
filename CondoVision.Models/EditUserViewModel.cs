using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class EditUserViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome completo não pode exceder 100 caracteres.")]
        [Display(Name = "Nome Completo")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email não é válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

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
        public int? CompanyId { get; set; }

        [Required(ErrorMessage = "A role é obrigatória.")]
        [Display(Name = "Perfil")]
        public string? RoleName { get; set; }
    }
}
