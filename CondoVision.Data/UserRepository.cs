using CondoVision.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CondoVision.Data
{

    public class UserRepository : IUserRepository
    {
        private readonly DbContext _context;
        public UserRepository(DbContext context)
        {
            _context = context;
        }

        public IQueryable<User> GetAllQueryable()
        {
            return _context.Set<User>().AsNoTracking().Where(u => !u.WasDeleted);
        }

        public async Task<User> GetByIdAsync(string id)
        {
            var user = await _context.Set<User>()
         .AsNoTracking()
         .FirstOrDefaultAsync(u => u.Id == id && !u.WasDeleted);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} was not found.");
            }
            return user;
        }

        public async Task AddAsync(User entity)
        {
            await _context.Set<User>().AddAsync(entity);
            await SaveChangesAsync();
        }

        public void Update(User entity)
        {
            _context.Set<User>().Update(entity);
        }

        public void Remove(User entity)
        {
            entity.WasDeleted = true;
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<User> CreateAsync(User entity)
        {
            await _context.Set<User>().AddAsync(entity);
            await SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(User entity)
        {
            _context.Set<User>().Update(entity);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(User entity)
        {
            entity.WasDeleted = true;
            _context.Entry(entity).State = EntityState.Modified;
            await SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Set<User>().AnyAsync(u => u.Id == id && !u.WasDeleted);
        }

        public async Task<string?> GetUserNameByIdAsync(string userId)
        {
            var user = await _context.Set<User>()
                .AsNoTracking()
                .Where(u => !u.WasDeleted)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user?.UserName;
        }
    }

}
