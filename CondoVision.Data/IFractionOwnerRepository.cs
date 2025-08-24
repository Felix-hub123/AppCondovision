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
        Task<IEnumerable<FractionOwner>> GetAllActiveAsync();

        Task CreateWithRelationsAsync(FractionOwner entity);


        Task UpdateWithRelationsAsync(FractionOwner entity);

    }
}
