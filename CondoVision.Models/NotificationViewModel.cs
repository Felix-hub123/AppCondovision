using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class NotificationViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Condominio")]
        public int CondominiumId { get; set; }

        [Display(Name = "Mensagem")]
        public string? Message { get; set; }

        [Display(Name = "Data da Notificação")]
        public DateTime NotificationDate { get; set; }

        [Display(Name = "Lida?")]
        public bool IsRead { get; set; }
        public bool WasDeleted { get; set; }
    }
}
