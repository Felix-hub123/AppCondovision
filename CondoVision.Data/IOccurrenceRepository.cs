using CondoVision.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface IOccurrenceRepository : IGenericRepository<Occurrence>
    {
        Task<IEnumerable<Occurrence>> GetOccurrencesByUnitIdAsync(int unitId, int? companyId);
        Task<Occurrence> GetByIdAsync(int id, int? companyId);
        Task<IEnumerable<Occurrence>> GetOpenOccurrencesAsync(int? companyId);

    }
}
