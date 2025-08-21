using CondoVision.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
       
        public CompanyRepository(DataContext context) : base(context)
        {
            
        }


        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<Company> AddCompanyAsync(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));
            await base.AddAsync(company);
            return company;
        }

        public async Task<Company> UpdateCompanyAsync(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));
            await base.UpdateAsync(company);
            return company;
        }

        public async Task DeleteCompanyAsync(int id)
        {
            var company = await base.GetByIdAsync(id, true); 
            if (company != null)
            {
                await base.DeleteAsync(company);
            }
            else
            {
                throw new KeyNotFoundException($"Company with ID {id} was not found.");
            }
        }

       
        public async Task<IEnumerable<Company>> GetActiveCompaniesAsync()
        {
            var companies = await base.GetAllAsync();
            return companies.Where(c => !c.WasDeleted && c.CreationDate > DateTime.Now.AddYears(-1));
        }

        public async Task<Company?> GetCompanyByTaxIdAsync(string taxId)
        {
            if (string.IsNullOrEmpty(taxId))
                throw new ArgumentException("Tax ID is required", nameof(taxId));
            var companies = await base.GetAllAsync();
            return companies.FirstOrDefault(c => c.CompanyTaxId == taxId && !c.WasDeleted);
        }

        public async Task<IEnumerable<Company>> GetCompaniesWithCondominiumsAsync()
        {
            return await _context.Companies
                .Include(c => c.Condominiums)
                .Where(c => !c.WasDeleted)
                .ToListAsync();
        }

    }
}
