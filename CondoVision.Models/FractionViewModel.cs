using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class FractionViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Número da Fração")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(20, ErrorMessage = "O campo {0} não pode ter mais de {1} caracteres.")]
        public string UnitNumber { get; set; } = string.Empty;

        [Display(Name = "Piso")]
        [MaxLength(10, ErrorMessage = "O campo {0} não pode ter mais de {1} caracteres.")]
        public string? Floor { get; set; }

        [Display(Name = "Bloco/Torre")]
        [MaxLength(10, ErrorMessage = "O campo {0} não pode ter mais de {1} caracteres.")]
        public string? Block { get; set; }

        [Display(Name = "Área (m²)")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Area { get; set; }

        [Display(Name = "Permilagem")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Permilage { get; set; }

        [Display(Name = "Condomínio")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Deve selecionar um condomínio.")]
        public int CondominiumId { get; set; }

        // Propriedade para exibir o nome do condomínio na vista
        [Display(Name = "Condomínio")]
        public string? CondominiumName { get; set; }

        // Propriedade para popular o dropdown de condomínios (para Create/Edit)
        public IEnumerable<SelectListItem>? Condominiums { get; set; }

        public bool WasDeleted { get; set; }
    }
}
