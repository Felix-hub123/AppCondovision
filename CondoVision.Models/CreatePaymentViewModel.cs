using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class CreatePaymentViewModel
    {
        [Required]
        [Display(Name = "Unidade")]
        public int UnitId { get; set; }

        [Required]
        [Display(Name = "Valor")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Data do Pagamento")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(200)]
        public string? Description { get; set; }
    }
}
