using CondoVision.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface IUnitRepository : IGenericRepository<Unit>
    {
        Task<IEnumerable<Unit>> GetUnitsByCondominiumIdAsync(int condominiumId, int? companyId = null);
        Task<Unit> GetByIdAsync(int id, int? companyId, bool ignoreSoftDelete = false);
        Task<IEnumerable<Unit>> GetAllActiveAsync();
        Task<bool> ExistsAsync(int id, int? companyId = null);
    }
}
