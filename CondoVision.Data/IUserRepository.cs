using CondoVision.Data.Entities;
using CondoVision.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface IUserRepository 
    {

        Task<IEnumerable<User>> GetAllAsync(int? companyId = null);

        IQueryable<User> GetAllQueryable(int? companyId = null);

        Task<User> GetByIdAsync(string id, int? companyId = null);

        Task AddAsync(User entity);

        void Update(User entity);

        void Remove(User entity);

        Task<int> SaveChangesAsync();

        Task<User> CreateAsync(User entity);

        Task UpdateAsync(User entity);

        Task DeleteAsync(User entity);

        Task<bool> ExistsAsync(string id, int? companyId = null);

        Task<string?> GetUserNameByIdAsync(string userId, int? companyId = null);

        Task<int> CompleteAsync();
    }
}
