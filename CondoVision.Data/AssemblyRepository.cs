using CondoVision.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public class AssemblyRepository : GenericRepository<Assembly>, IAssemblyRepository
    {
        public AssemblyRepository(DataContext context) : base(context)
        {
            
        }


        public async Task<IEnumerable<Assembly>> GetAssembliesByCondominiumIdAsync(int condominiumId, int? companyId)
        {
            return await _context.Assemblies
                .Include(a => a.Condominium)
                .Where(a => a.CondominiumId == condominiumId && a.Condominium!.CompanyId == companyId && !a.WasDeleted)
                .ToListAsync();
        }

        public async Task<Assembly> GetByIdAsync(int id, int? companyId)
        {
            return await _context.Assemblies
                .Include(a => a.Condominium)
                .FirstOrDefaultAsync(a => a.Id == id && a.Condominium.CompanyId == companyId && !a.WasDeleted);
        }

        public async Task<IEnumerable<Assembly>> GetPublishedAssembliesAsync(int? companyId)
        {
            return await _context.Assemblies
                .Include(a => a.Condominium)
                .Where(a => a.IsPublished && a.Condominium!.CompanyId == companyId && !a.WasDeleted)
                .ToListAsync();
        }
    }

}
