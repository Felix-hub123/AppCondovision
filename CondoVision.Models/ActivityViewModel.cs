using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class ActivityViewModel
    {
        public int Id { get; set; }


        [Display(Name = "Nome do Utilizador")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Ação")]
        public string Action { get; set; } = string.Empty;

        [Display(Name = "Data")]
        public DateTime Date { get; set; }
    }
}
