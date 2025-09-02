using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace CondoVision.Models.Entities
{
    public class CondominiumRepository : GenericRepository<Condominium>, ICondominiumRepository
    {
        public CondominiumRepository(DataContext context) : base(context)
        {

        }

        public async Task<IEnumerable<Condominium>> GetAllActiveAsync(int? companyId = null)
        {
            var query = _context.Condominiums.Where(c => !c.WasDeleted);
            if (companyId.HasValue)
            {
                query = query.Where(c => c.CompanyId == companyId);
            }
            return await query.ToListAsync();
        }

        public Task<List<Condominium>> GetCondominiumsByCompanyIdAsync(int companyId)
        {
            if (companyId <= 0)
            {
                throw new ArgumentException("O ID da empresa deve ser maior que zero.", nameof(companyId));
            }

            return _context.Condominiums
                .Where(c => c.CompanyId == companyId && !c.WasDeleted)
                .Include(c => c.Address)
                .ToListAsync();
        }

        public async Task<IEnumerable<Condominium>> GetAllCondominiumsWithCompanyAsync(int? companyId = null)
        {
            var query = _context.Condominiums
                .Include(c => c.Company)
                .Where(c => !c.WasDeleted);
            if (companyId.HasValue)
            {
                query = query.Where(c => c.CompanyId == companyId);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Condominium>> GetAllCondominiumsAsync(int? companyId = null)
        {
            var query = _context.Condominiums.Where(c => !c.WasDeleted);
            if (companyId.HasValue)
            {
                query = query.Where(c => c.CompanyId == companyId);
            }
            return await query.ToListAsync();
        }

        public async Task<string?> GetCondominiumNameByIdAsync(int id, int? companyId = null)
        {
            var condo = await GetByIdAsync(id, companyId);
            return condo?.Name;
        }

        public async Task<Condominium?> GetByIdAsync(int id, int? companyId = null)
        {
            var query = _context.Condominiums.Where(c => !c.WasDeleted);
            if (companyId.HasValue)
            {
                query = query.Where(c => c.CompanyId == companyId);
            }
            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Condominium?> GetCondominiumByIdAsync(int id, int? companyId = null)
        {
            return await GetByIdAsync(id, companyId);
        }

        public async Task<Condominium> AddCondominiumAsync(Condominium condominium)
        {
            if (condominium == null)
                throw new ArgumentNullException(nameof(condominium));
            await base.AddAsync(condominium);
            return condominium;
        }

        public async Task<Condominium> UpdateCondominiumAsync(Condominium condominium)
        {
            if (condominium == null)
                throw new ArgumentNullException(nameof(condominium));
            await base.UpdateAsync(condominium);
            return condominium;
        }

        public async Task DeleteCondominiumAsync(int id)
        {
            var condominium = await GetByIdAsync(id);
            if (condominium != null)
            {
                await base.DeleteAsync(condominium);
            }
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}
