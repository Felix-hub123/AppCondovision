using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class EditUnitViewModel
    {
        [Required(ErrorMessage = "O nome da unidade é obrigatório.")]
        [Display(Name = "Nome da Unidade")]
        public string? UnitName { get; set; }

        [Required(ErrorMessage = "A permilagem é obrigatória.")]
        [Range(0.01, 100.00, ErrorMessage = "A permilagem deve estar entre 0.01 e 100.00.")]
        [Display(Name = "Permilagem")]
        public decimal Permillage { get; set; }

        [Display(Name = "Proprietário")]
        public string? OwnerId { get; set; } 

        [Display(Name = "Condomínio")]
        [Required(ErrorMessage = "O ID do condomínio é obrigatório.")]
        public int CondominiumId { get; set; }

        [Display(Name = "ID da Unidade")]
        public int Id { get; set; }
    }
}
