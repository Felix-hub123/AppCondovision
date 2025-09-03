using CondoVision.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface IAssemblyRepository : IGenericRepository<Assembly>
    {
        Task<IEnumerable<Assembly>> GetAssembliesByCondominiumIdAsync(int condominiumId, int? companyId);
        Task<Assembly> GetByIdAsync(int id, int? companyId);
        Task<IEnumerable<Assembly>> GetPublishedAssembliesAsync(int? companyId);

    }
}
