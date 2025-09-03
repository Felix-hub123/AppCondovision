using CondoVision.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public class UnitRepository : GenericRepository<Unit>, IUnitRepository
    {
        public UnitRepository(DataContext context) : base(context)
        {
        }



        public async Task<IEnumerable<Unit>> GetAllActiveAsync(int? companyId)
        {
            return await _context.Units
                .Include(u => u.Condominium)
                .Where(u => !u.WasDeleted && u.Condominium.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<Unit> GetByIdAsync(int id, int? companyId)
        {
            return await _context.Units
                .Include(u => u.Condominium)
                .FirstOrDefaultAsync(u => u.Id == id && u.Condominium.CompanyId == companyId && !u.WasDeleted);
        }
        public new Task<Unit> GetByIdAsync(int id, bool ignoreSoftDelete = false)
        {
            return base.GetByIdAsync(id, ignoreSoftDelete);
        }

        public async Task<IEnumerable<Unit>> GetAllAsync(int? companyId = null)
        {
            var query = _context.Units.AsQueryable();
            if (companyId.HasValue)
            {
                query = query.Where(u => u.CompanyId == companyId);
            }
            return await query.ToListAsync();
        }
        public async Task<Unit> GetByIdAsync(int id, int? companyId, bool ignoreSoftDelete = false)
        {
            var query = _context.Set<Unit>().AsNoTracking();
            if (!ignoreSoftDelete)
            {
                query = query.Where(u => !u.WasDeleted);
            }

            if (companyId.HasValue)
            {
                query = query.Where(u => u.CompanyId == companyId);
            }

            var unit = await query.FirstOrDefaultAsync(u => u.Id == id);
            if (unit == null)
            {
                throw new KeyNotFoundException($"Unidade com ID {id} não encontrada para CompanyId {companyId}.");
            }
            return unit;
        }

        public async Task<IEnumerable<Unit>> GetUnitsByCondominiumIdAsync(int condominiumId, int? companyId = null)
        {
            var query = _context.Set<Unit>().AsNoTracking()
                .Where(u => !u.WasDeleted && u.CondominiumId == condominiumId);

            if (companyId.HasValue)
            {
                query = query.Where(u => u.CompanyId == companyId);
            }

            return await query.OrderBy(u => u.Id).ToListAsync(); 
        }

        public async Task<IEnumerable<Unit>> GetAllActiveAsync()
        {
            return await _context.Set<Unit>().Where(u => !u.WasDeleted).ToListAsync();
        }

        public new async Task AddAsync(Unit entity)
        {
            await base.AddAsync(entity);
        }

        public new async Task UpdateAsync(Unit entity)
        {
            await base.UpdateAsync(entity);
        }

        public new async Task DeleteAsync(Unit entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await base.DeleteAsync(entity); 
        }

        public async Task<bool> ExistsAsync(int id, int? companyId = null)
        {
            var query = _context.Set<Unit>().Where(u => u.Id == id && !u.WasDeleted);
            if (companyId.HasValue)
            {
                query = query.Where(u => u.CompanyId == companyId);
            }
            return await query.AnyAsync();
        }

    }
}
