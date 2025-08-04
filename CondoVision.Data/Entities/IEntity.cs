using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data.Data
{
    public interface IEntity
    {
        int Id { get; set; }


        bool WasDeleted { get; set; }

    }
}
