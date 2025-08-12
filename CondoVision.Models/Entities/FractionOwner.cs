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
    public class FractionOwner : IEntity
    {
        public int Id { get; set; } 

     
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Utilizador (Proprietário)")]
        public string UserId { get; set; } = string.Empty; 

        [ForeignKey("UserId")]
        public User? User { get; set; } 

       
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Fração")]
        public int FractionId { get; set; }

        [ForeignKey("FractionId")]
        public Fraction? Fraction { get; set; } 

       

        public bool WasDeleted { get; set; } 
    }
}
