using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class CreateExpenseViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Condomínio")]
        public int CondominiumId { get; set; } 


        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(200, ErrorMessage = "A {0} não pode exceder {1} caracteres.")]
        [Display(Name = "Descrição")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O {0} deve ser um valor positivo.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Valor")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DataType(DataType.Date, ErrorMessage = "Formato de data inválido.")]
        [Display(Name = "Data da Despesa")]
        public DateTime ExpenseDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50, ErrorMessage = "A {0} não pode exceder {1} caracteres.")]
        [Display(Name = "Categoria")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50, ErrorMessage = "O {0} não pode exceder {1} caracteres.")] 
        [Display(Name = "Estado")]
        public string Status { get; set; } = "Pendente"; 
    }
}
