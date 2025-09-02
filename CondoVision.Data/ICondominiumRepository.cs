using CondoVision.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface ICondominiumRepository : IGenericRepository<Condominium>
    {
        Task<IEnumerable<Condominium>> GetAllActiveAsync(int? companyId = null);

        Task<List<Condominium>> GetCondominiumsByCompanyIdAsync(int companyId);

        Task<IEnumerable<Condominium>> GetAllCondominiumsWithCompanyAsync(int? companyId = null);

        Task<IEnumerable<Condominium>> GetAllCondominiumsAsync(int? companyId = null);

        Task<string?> GetCondominiumNameByIdAsync(int id, int? companyId = null);

        Task<Condominium?> GetByIdAsync(int id, int? companyId = null);

        Task<Condominium?> GetCondominiumByIdAsync(int id, int? companyId = null);

        Task<Condominium> AddCondominiumAsync(Condominium condominium);

        Task<Condominium> UpdateCondominiumAsync(Condominium condominium);

        Task DeleteCondominiumAsync(int id);

        Task CompleteAsync();
    }


}

