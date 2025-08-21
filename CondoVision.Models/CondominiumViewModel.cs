using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class CondominiumViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(255, ErrorMessage = "O nome não pode exceder 255 caracteres.")]
        [Display(Name = "Nome do Condomínio")]
        public string? Name { get; set; }

        [StringLength(255, ErrorMessage = "A morada não pode exceder 255 caracteres.")]
        [Display(Name = "Morada")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "A cidade não pode exceder 100 caracteres.")]
        [Display(Name = "Cidade")]
        public string? City { get; set; }

        [StringLength(20, ErrorMessage = "O código postal não pode exceder 20 caracteres.")]
        [Display(Name = "Código Postal")]
        public string? PostalCode { get; set; }

        [Required(ErrorMessage = "A empresa é obrigatória.")]
        [Display(Name = "Empresa Associada")]
        public int? CompanyId { get; set; }

        [Display(Name = "Nome da Empresa")]
        public string? CompanyName { get; set; }

        [Display(Name = "Data de Registo")]
        public DateTime RegistrationDate { get; set; }

        [Display(Name = "Excluído")]
        public bool WasDeleted { get; set; }
    }
}
