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

        public async Task<IEnumerable<User>> GetAllAsync(int? companyId = null)
        {
            var query = _context.Users.AsQueryable();
            if (companyId.HasValue)
            {
                query = query.Where(u => u.CompanyId == companyId);
            }
            return await query.ToListAsync();
        }

        public IQueryable<User> GetAllQueryable(int? companyId = null)
        {
            var query = _context.Set<User>().AsNoTracking().Where(u => !u.WasDeleted);
            if (companyId.HasValue)
            {
                query = query.Where(u => u.CompanyId == companyId);
            }
            return query;
        }

        public async Task<User> GetByIdAsync(string id, int? companyId = null)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var query = _context.Set<User>().AsNoTracking().Where(u => !u.WasDeleted);
            if (companyId.HasValue) query = query.Where(u => u.CompanyId == companyId);

            return await query.FirstOrDefaultAsync(u => u.Id == id) ?? throw new KeyNotFoundException($"User with ID {id} was not found.");
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
            {
                throw new ArgumentNullException(nameof(entity));
            }
                

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
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            entity.WasDeleted = true;
            _context.Entry(entity).State = EntityState.Modified;
            await SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string id, int? companyId = null)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            var query = _context.Set<User>().Where(u => !u.WasDeleted);
            if (companyId.HasValue)
            {
                query = query.Where(u => u.CompanyId == companyId);
            }
            return await query.AnyAsync(u => u.Id == id);
        }

        public async Task<string?> GetUserNameByIdAsync(string userId, int? companyId = null)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var query = _context.Set<User>()
                .AsNoTracking()
                .Where(u => !u.WasDeleted);
            if (companyId.HasValue)
            {
                query = query.Where(u => u.CompanyId == companyId);
            }

            var user = await query.FirstOrDefaultAsync(u => u.Id == userId);
            return user?.UserName;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }

}
