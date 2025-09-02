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



        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            return await _context.Companies
                .Include(c => c.Condominiums)
                .FirstOrDefaultAsync(c => c.Id == id && !c.WasDeleted);
        }

        public async Task<Company> AddCompanyAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<Company> UpdateCompanyAsync(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }
            _context.Companies.Update(company); 
            await _context.SaveChangesAsync();  
            return company;
        }

        public async Task DeleteCompanyAsync(int id)
        {
            var company = await base.GetByIdAsync(id, true);
            if (company == null)
            {
                throw new KeyNotFoundException($"Company with ID {id} was not found.");
            }
            if (await _context.Users.AnyAsync(u => u.CompanyId == id))
            {
                throw new InvalidOperationException("Cannot delete company with associated users.");
            }
            company.WasDeleted = true; 
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Company>> GetActiveCompaniesAsync()
        {
            return await _context.Companies
                .Where(c => !c.WasDeleted && c.CreationDate > DateTime.Now.AddYears(-1))
                .ToListAsync();
        }

        public async Task<IEnumerable<Company>> GetCompaniesByCompanyIdAsync(int companyId)
        {
            return await _context.Companies
                .Where(c => c.Id == companyId && !c.WasDeleted)
                .ToListAsync();
        }

        public async Task<Company?> GetCompanyByTaxIdAsync(string taxId)
        {
            if (string.IsNullOrEmpty(taxId))
            {
                throw new ArgumentException("Tax ID is required", nameof(taxId));
            }
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.CompanyTaxId == taxId && !c.WasDeleted);
        }

        public async Task<IEnumerable<Company>> GetCompaniesWithCondominiumsAsync()
        {
            return await _context.Companies
                .Include(c => c.Condominiums)
                .Where(c => !c.WasDeleted)
                .ToListAsync();
        }

        public async Task<List<Company>> GetCompaniesWithCondominiumsAsync(int? companyId = null)
        {
            var query = _context.Companies
                .Include(c => c.Condominiums)
                .Where(c => !c.WasDeleted);
            if (companyId.HasValue)
            {
                query = query.Where(c => c.Id == companyId.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<bool> IsTaxIdUniqueAsync(string taxId, int? excludeId = null)
        {
            var query = _context.Companies.Where(c => c.CompanyTaxId == taxId && !c.WasDeleted);
            if (excludeId.HasValue) query = query.Where(c => c.Id != excludeId.Value);
            return !await query.AnyAsync();
        }


    }
}
