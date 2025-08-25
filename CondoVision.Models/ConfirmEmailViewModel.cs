using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class ConfirmEmailViewModel
    {
        [Display (Name = "Sucesso?")]
        public bool IsSuccess { get; set; } 

        [Display(Name = "Mensagem")]
        public string? Message { get; set; } 

        [Display(Name = "E-mail")]
        public string? Email { get; set; } 

        [Display(Name = "URL de Retorno")]
        public string? ReturnUrl { get; set; }
    }
}
