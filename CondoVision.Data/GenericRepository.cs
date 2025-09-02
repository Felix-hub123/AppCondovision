using CondoVision.Models.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace CondoVision.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        protected readonly DataContext _context;

        public GenericRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await GetAllQueryable().ToListAsync();
        }


        public IQueryable<T> GetAllQueryable()
        {
            return _context.Set<T>().AsNoTracking().Where(e => !e.WasDeleted);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await GetByIdAsync(id, false); 
        }


        public async Task<T> GetByIdAsync(int id, bool ignoreSoftDelete = false)
        {
            var query = _context.Set<T>().AsNoTracking();
            if (!ignoreSoftDelete)
            {
                query = query.Where(e => !e.WasDeleted);
            }
            var entity = await query.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity of type {typeof(T).Name} with ID {id} was not found.");
            }
            return entity;
        }


        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveChangesAsync();
        }

    
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }


        public void Remove(T entity)
        {
            entity.WasDeleted = true; 
            _context.Entry(entity).State = EntityState.Modified;
        }

     
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

     
        public async Task<T> CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            var existingEntity = await _context.Set<T>().FindAsync(entity.Id); 
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity); 
            }
            else
            {
                _context.Set<T>().Update(entity); 
            }
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            var existingEntity = await _context.Set<T>().FindAsync(entity.Id); 
            if (existingEntity != null)
            {
                existingEntity.WasDeleted = true;
                _context.Entry(existingEntity).State = EntityState.Modified;
            }
            await SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<T>().AnyAsync(e => e.Id == id && !e.WasDeleted);
        }

        public async Task<List<T>> GetRecentAsync(int take = 5)
        {
            return await GetAllQueryable()
                .OrderByDescending(e => EF.Property<DateTime>(e, "CreatedAt")) 
                .Take(take)
                .ToListAsync();
        }


    }


}
