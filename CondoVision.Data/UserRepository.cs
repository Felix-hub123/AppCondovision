using CondoVision.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CondoVision.Data
{

    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync(); 
        }

        public IQueryable<User> GetAllQueryable()
        {
            return _context.Set<User>().AsNoTracking().Where(u => !u.WasDeleted);
        }

        public async Task<User> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

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
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _context.Set<User>().AddAsync(entity);
            await SaveChangesAsync();
        }

        public void Update(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<User>().Update(entity);
        }

        public void Remove(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.WasDeleted = true;
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<User> CreateAsync(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _context.Set<User>().AddAsync(entity);
            await SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<User>().Update(entity);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.WasDeleted = true;
            _context.Entry(entity).State = EntityState.Modified;
            await SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            return await _context.Set<User>().AnyAsync(u => u.Id == id && !u.WasDeleted);
        }

        public async Task<string?> GetUserNameByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var user = await _context.Set<User>()
                .AsNoTracking()
                .Where(u => !u.WasDeleted)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.UserName;
        }
    }

}
