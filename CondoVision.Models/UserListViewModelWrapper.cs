using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class UserListViewModelWrapper
    {
        public List<UserListViewModel> Items { get; set; } = new List<UserListViewModel>();

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }
    }
}
