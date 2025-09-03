using CondoVision.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface IFractionOwnerRepository : IGenericRepository<FractionOwner>
    {
        Task<IEnumerable<FractionOwner>> GetAllActiveAsync(int? companyId);
        Task<FractionOwner> GetByIdAsync(int id, int? companyId);
        Task<FractionOwner> GetByUserIdAsync(string userId, int? companyId);
        Task CreateWithRelationsAsync(FractionOwner fractionOwner);
        Task UpdateWithRelationsAsync(FractionOwner fractionOwner);

    }
}
