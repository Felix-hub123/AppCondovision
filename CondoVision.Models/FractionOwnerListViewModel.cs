using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class FractionOwnerListViewModel
    {
        public List<FractionOwnerViewModel>? FractionOwners { get; set; }
        public int TotalCount { get; set; }
    }
}
