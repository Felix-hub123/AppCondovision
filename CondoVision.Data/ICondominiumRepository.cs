using CondoVision.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface ICondominiumRepository : IGenericRepository<Condominium>
    {
        Task<IEnumerable<Condominium>> GetCondominiumsByUser(string userId);
        Task<IEnumerable<Condominium>> GetActiveCondominiums();
    }
}
