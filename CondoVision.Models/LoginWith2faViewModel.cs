using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class LoginWith2faViewModel
    {
        [Required]
        [Display(Name = "Código de Verificação")]
        public string? TwoFactorCode { get; set; }

        [Display(Name = "Lembrar-me nesta máquina")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }
}
