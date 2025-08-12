using CondoVision.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data.Entities
{
    public class Condominium : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do condomínio é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string? Name { get; set; }


        [Required(ErrorMessage = "A morada do condomínio é obrigatória.")]
        [StringLength(200, ErrorMessage = "A morada não pode exceder 200 caracteres.")]
        public string? Address { get; set; }

        [Display(Name = "Data de Registo")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationDate { get; set; }

        public DateTime CreationDate { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company? Company { get; set; }

        public bool WasDeleted { get; set; }

        public ICollection<User>? Users { get; set; }

        public virtual ICollection<Unit>? Units { get; set; }
    }
}
