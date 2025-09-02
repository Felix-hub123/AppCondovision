using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class ManageUsersViewModel
    {
        public int CompanyId { get; set; }
        public List<UserRoleViewModel> Users { get; set; } = new List<UserRoleViewModel>();
    }
}
