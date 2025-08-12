using CondoVision.Data.Entities;
using CondoVision.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models.Entities
{
    public class Fraction : IEntity
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
        [Column(TypeName = "decimal(18,2)")] // Define a precisão para valores decimais
        public decimal Area { get; set; }

        [Display(Name = "Permilagem")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Column(TypeName = "decimal(18,4)")] // Permilagem pode ter mais casas decimais
        public decimal Permilage { get; set; }

        // Chave estrangeira para o Condomínio
        [Display(Name = "Condomínio")]
        public int CondominiumId { get; set; }

        public ICollection<FractionOwner>? FractionOwners { get; set; }

        // Propriedade de navegação para o Condomínio
        public Condominium? Condominium { get; set; }

        // Propriedade para soft delete
        public bool WasDeleted { get; set; }
    }
}
