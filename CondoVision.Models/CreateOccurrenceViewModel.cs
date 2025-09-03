using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class CreateOccurrenceViewModel
    {
        [Display(Name = "Unidade")]
        public int UnitId { get; set; }

        [Display(Name = "Título")]
        public string? Description { get; set; }

        [Display(Name = "Data da ocorrencia")]
        public DateTime? OccurrenceDate { get; set; }
    }
}
