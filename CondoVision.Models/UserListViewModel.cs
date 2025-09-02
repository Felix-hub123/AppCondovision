using CondoVision.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class UserListViewModel
    {

        public string? Id { get; set; }

        [Display(Name = "Nome Completo")]
        public string? FullName { get; set; }

        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [Display(Name = "Nome de Usuário")]
        public string? PhoneNumber { get; set; }

        public List<User>? Items { get; set; }
              
        public string? RoleName { get; set; }
    }
}
