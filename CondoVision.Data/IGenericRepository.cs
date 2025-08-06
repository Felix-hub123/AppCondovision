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
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity); 
        void Update(T entity); 
        void Delete(T entity); 
        Task<int> CompleteAsync();

        IQueryable<T> GetQueryable();

        Task DeleteAsync(int id);

        Task<IEnumerable<T>> GetAllAsync(IQueryable<T> query);

        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includeExpressions);

        Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includeExpressions);
    }


}
   
    

