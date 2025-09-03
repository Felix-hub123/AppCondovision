using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class AssemblyViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Titulo da assembleia")]
        public string ?Title { get; set; }

        [Display(Name = "Data da assembleia")]
        public DateTime AssemblyDate { get; set; }

        [Display(Name = "Condominio")]
        public int CondominiumId { get; set; }

        [Display(Name = "Minuto")]
        public string? Minutes { get; set; }

        [Display(Name = "Publicado?")]
        public bool IsPublished { get; set; }

        public bool WasDeleted { get; set; }
    }
}
