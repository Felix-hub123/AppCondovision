using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class FractionOwnerViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Número da Fração")]
        public string UnitNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Unidade")]
        public int UnitId { get; set; }

        [Display(Name = "Piso da Fração")]
        public string? FractionFloor { get; set; }

        [Display(Name = "Bloco da Fração")]
        public string? FractionBlock { get; set; }

        [Display(Name = "Condomínio da Fração")]
        public string? CondominiumName { get; set; }

        [Required]
        [Display(Name = "Usuário Proprietário")] 
        public string UserId { get; set; } = string.Empty;
    }
}
