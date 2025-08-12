using CondoVision.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public class FractionOwnerRepository : GenericRepository<FractionOwner>, IFractionOwnerRepository
    {
        private readonly DataContext _dataContext;

        public FractionOwnerRepository(DataContext context) : base(context)
        {
            _dataContext = context;
        }

        public async Task<IEnumerable<FractionOwner>> GetAllFractionOwnersWithDetailsAsync()
        {
            return await _context.FractionOwners
                                 .Include(fo => fo.Fraction)
                                     .ThenInclude(f => f!.Condominium) 
                                 .Include(fo => fo.User)
                                 .Where(fo => !fo.WasDeleted) 
                                 .ToListAsync();
        }

        public async Task<FractionOwner?> GetFractionOwnerWithDetailsAsync(int id)
        {
            return await _context.FractionOwners
                                 .Include(fo => fo.Fraction)
                                     .ThenInclude(f => f!.Condominium)
                                 .Include(fo => fo.User)
                                 .Where(fo => fo.Id == id && !fo.WasDeleted) // Aplica filtro de soft delete
                                 .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<FractionOwner>> GetFractionOwnersByFractionIdAsync(int fractionId)
        {
            return await _context.FractionOwners
                                 .Include(fo => fo.User)
                                 .Where(fo => fo.FractionId == fractionId && !fo.WasDeleted)
                                 .ToListAsync();
        }


        public async Task<IEnumerable<FractionOwner>> GetFractionOwnersByUserIdAsync(string userId)
        {
            return await _context.FractionOwners
                                 .Include(fo => fo.Fraction)
                                     .ThenInclude(f => f!.Condominium)
                                 .Where(fo => fo.UserId == userId && !fo.WasDeleted)
                                 .ToListAsync();
        }

        public async Task<bool> FractionOwnerExistsAsync(int fractionId, string userId)
        {
            return await _context.FractionOwners
                                 .AnyAsync(fo => fo.FractionId == fractionId && fo.UserId == userId && !fo.WasDeleted);
        }

    }
}
