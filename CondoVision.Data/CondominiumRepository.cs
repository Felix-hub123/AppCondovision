using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CondoVision.Models.Entities
{
    public class CondominiumRepository : GenericRepository<Condominium>, ICondominiumRepository
    {
        public CondominiumRepository(DataContext context) : base(context)
        {

        }

        public async Task<IEnumerable<Condominium>> GetCondominiumsByUser(string userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.CompanyId == null)
            {
                return Enumerable.Empty<Condominium>();
            }

            return await _dbSet
                .Where(c => c.CompanyId == user.CompanyId && !c.WasDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Condominium>> GetActiveCondominiums()
        {
            return await _dbSet.Where(c => !c.WasDeleted).ToListAsync();
        }

        public async Task<List<Condominium>> GetAllWithCompaniesAsync()
        {
            return await GetQueryable().Include(c => c.Company).ToListAsync();
        }

        public async Task<IEnumerable<Condominium>> GetAllCondominiumsWithCompanyAsync()
        {
            return await this.GetAllWithIncludesAsync(c => c.Company);
        }

        public async Task<Condominium?> GetCondominiumWithCompanyAsync(int id)
        {
            return await this.GetByIdWithIncludesAsync(id, c => c.Company);
        }
    }
}
