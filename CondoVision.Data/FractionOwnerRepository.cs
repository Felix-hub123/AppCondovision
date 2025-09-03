using CondoVision.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public class FractionOwnerRepository :GenericRepository<FractionOwner>, IFractionOwnerRepository
    {
        public FractionOwnerRepository(DataContext context) : base(context)
        {
        }


        public async Task<IEnumerable<FractionOwner>> GetAllActiveAsync(int? companyId)
        {
            return await _context.FractionOwners
                .Include(fo => fo.Unit)
                .Include(fo => fo.User)
                .Where(fo => !fo.WasDeleted && fo.Unit.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<FractionOwner> GetByIdAsync(int id, int? companyId)
        {
            return await _context.FractionOwners
                .Include(fo => fo.Unit)
                .Include(fo => fo.User)
                .FirstOrDefaultAsync(fo => fo.Id == id && fo.Unit.CompanyId == companyId && !fo.WasDeleted);
        }

        public async Task<FractionOwner> GetByUserIdAsync(string userId, int? companyId)
        {
            return await _context.FractionOwners
                .Include(fo => fo.Unit)
                .Include(fo => fo.User)
                .FirstOrDefaultAsync(fo => fo.UserId == userId && fo.Unit.CompanyId == companyId && !fo.WasDeleted);
        }

        public async Task CreateWithRelationsAsync(FractionOwner fractionOwner)
        {
            await _context.FractionOwners.AddAsync(fractionOwner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWithRelationsAsync(FractionOwner fractionOwner)
        {
            _context.FractionOwners.Update(fractionOwner);
            await _context.SaveChangesAsync();
        }
    }



}

