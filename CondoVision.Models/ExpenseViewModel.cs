using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class ExpenseViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Condomínio")]
        public string CondominiumName { get; set; } = string.Empty; 

        [Display(Name = "Descrição")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Criado Por")]
        public string UserName { get; set; } = string.Empty; 

        [Display(Name = "Valor")]
        public string AmountFormatted { get; set; } = string.Empty; 
        [Display(Name = "Data da Despesa")]
        public string ExpenseDateFormatted { get; set; } = string.Empty; 

        [Display(Name = "Categoria")]
        public string Category { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Removido")]
        public bool WasDeleted { get; set; }
    }
}
