using CondoVision.Models.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CondoVision.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        protected readonly DataContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DataContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }


        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.Where(e => !e.WasDeleted).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id) 
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.WasDeleted);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            entity.WasDeleted = true;
            Update(entity);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }


}
