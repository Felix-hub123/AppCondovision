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
        Task<List<Condominium>> GetCondominiumsByCompanyIdAsync(int companyId);
        Task<IEnumerable<Condominium>> GetAllActiveAsync();
        Task<string?> GetCondominiumNameByIdAsync(int id);

        Task<IEnumerable<Condominium>> GetAllCondominiumsWithCompanyAsync();

        Task<IEnumerable<Condominium>> GetAllCondominiumsAsync();

        Task<Condominium?> GetCondominiumByIdAsync(int id);

        Task<Condominium> AddCondominiumAsync(Condominium condominium);

        Task<Condominium> UpdateCondominiumAsync(Condominium condominium);

        Task DeleteCondominiumAsync(int id);

        Task CompleteAsync();
    }


}

