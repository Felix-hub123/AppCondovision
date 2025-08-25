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

        public async Task<IEnumerable<Condominium>> GetAllActiveAsync()
        {
            return await _context.Condominiums
                .Where(c => !c.WasDeleted)
                .ToListAsync();
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


        // Em CondominiumRepository.cs
        public async Task<IEnumerable<Condominium>> GetAllCondominiumsWithCompanyAsync()
        {
            return await _context.Condominiums
                .Include(c => c.Company) // Inclui a relação com Company
                .Where(c => !c.WasDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Condominium>> GetAllCondominiumsAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<string?> GetCondominiumNameByIdAsync(int id)
        {
            return (await GetByIdAsync(id))?.Name;
        }

        public async Task<Condominium?> GetCondominiumByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
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
            var condominium = await base.GetByIdAsync(id, true); 
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
