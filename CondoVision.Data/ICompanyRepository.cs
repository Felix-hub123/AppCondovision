using CondoVision.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<IEnumerable<Company>> GetAllCompaniesAsync();

        Task<Company> AddCompanyAsync(Company company);

        Task<Company?> GetCompanyByIdAsync(int id);

        Task<Company> UpdateCompanyAsync(Company company);

        Task DeleteCompanyAsync(int id);


        Task<IEnumerable<Company>> GetActiveCompaniesAsync();

        Task<Company?> GetCompanyByTaxIdAsync(string taxId);
        /// <summary>
        /// Obtém todas as empresas com seus condomínios associados de forma assíncrona.
        /// </summary>
        /// <returns>Uma coleção de empresas com condomínios.</returns>
        Task<IEnumerable<Company>> GetCompaniesWithCondominiumsAsync();
    }
}
