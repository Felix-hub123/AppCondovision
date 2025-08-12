using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class AssociateFractionOwnerViewModel
    {
        public int Id { get; set; } 

        [Display(Name = "Fração")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Deve selecionar uma fração.")]
        public int FractionId { get; set; }

     
        public IEnumerable<SelectListItem>? Fractions { get; set; }

        [Display(Name = "Proprietário (Utilizador)")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]

        public string? UserId { get; set; }


        public IEnumerable<SelectListItem>? Users { get; set; }


        public string? CurrentFractionDetails { get; set; }
        public string? CurrentOwnerDetails { get; set; }

    }
}
