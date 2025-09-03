using CondoVision.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public class OccurrencyRepository : GenericRepository<Occurrence>, IOccurrenceRepository
    {
        public OccurrencyRepository(DataContext context) : base(context)
        { 

        }

        public async Task<IEnumerable<Occurrence>> GetOccurrencesByUnitIdAsync(int unitId, int? companyId)
        {
            return await _context.Occurrences
                .Include(o => o.Unit)
                .Include(o => o.FractionOwner)
                .Where(o => o.UnitId == unitId && o.Unit!.CompanyId == companyId && !o.WasDeleted)
                .ToListAsync();
        }

        public async Task<Occurrence> GetByIdAsync(int id, int? companyId)
        {
            return await _context.Occurrences
                .Include(o => o.Unit)
                .Include(o => o.FractionOwner)
                .FirstOrDefaultAsync(o => o.Id == id && o.Unit!.CompanyId == companyId && !o.WasDeleted);
        }

        public async Task<IEnumerable<Occurrence>> GetOpenOccurrencesAsync(int? companyId)
        {
            return await _context.Occurrences
                .Include(o => o.Unit)
                .Where(o => o.Status == "Aberta" && o.Unit!.CompanyId == companyId && !o.WasDeleted)
                .ToListAsync();
        }
    }
}

