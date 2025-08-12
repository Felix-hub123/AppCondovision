using CondoVision.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface IFractionRepository : IGenericRepository<Fraction>
    {
        Task<IEnumerable<Fraction>> GetAllFractionsWithCondominiumAsync();

        Task<Fraction?> GetFractionWithCondominiumAsync(int id);

    }
}
