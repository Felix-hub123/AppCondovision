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
        Task<IEnumerable<FractionOwner>> GetAllFractionOwnersWithDetailsAsync();

        Task<FractionOwner?> GetFractionOwnerWithDetailsAsync(int id);

        Task<IEnumerable<FractionOwner>> GetFractionOwnersByFractionIdAsync(int fractionId);

        Task<IEnumerable<FractionOwner>> GetFractionOwnersByUserIdAsync(string userId);

        Task<bool> FractionOwnerExistsAsync(int fractionId, string userId);
    }
}
