using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class CompanyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da empresa é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da empresa não pode exceder 100 caracteres.")]
        [Display(Name = "Nome da Empresa")]
        public string? Name { get; set; }

        [StringLength(20, ErrorMessage = "O contribuinte não pode exceder 20 caracteres.")]
        [Display(Name = "Contribuinte")]
        public string? CompanyTaxId { get; set; }

        [StringLength(200, ErrorMessage = "A morada não pode exceder 200 caracteres.")]
        [Display(Name = "Morada")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "O contacto não pode exceder 100 caracteres.")]
        [Display(Name = "Contacto")]
        public string? Contact { get; set; }

        [Display(Name = "ID do Logotipo")]
        public Guid? LogoId { get; set; }

        [Required(ErrorMessage = "A data de criação é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Criação")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Eliminado")]
        public bool WasDeleted { get; set; }

        [Display(Name = "Condominios")]
        public List<CondominiumViewModel>? Condominiums { get; set; }

        [Display(Name = "Companinha")]
        public int? CompanyId { get; set; }
    }
}
