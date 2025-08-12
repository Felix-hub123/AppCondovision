using CondoVision.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace CondoVision.Data
{
    public class FractionRepository : GenericRepository<Fraction>, IFractionRepository
    {
        private readonly DataContext _dataContext;

        public FractionRepository(DataContext dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IEnumerable<Fraction>> GetAllFractionsWithCondominiumAsync()
        {
            return await _context.Fractions
                                 .Include(f => f.Condominium) 
                                 .OrderBy(f => f.Condominium!.Name) 
                                 .ThenBy(f => f.UnitNumber) 
                                 .ToListAsync();
        }

        public async Task<Fraction?> GetFractionWithCondominiumAsync(int id)
        {
            return await _context.Fractions
                                 .Include(f => f.Condominium)
                                 .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
