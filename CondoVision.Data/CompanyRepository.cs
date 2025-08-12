using CondoVision.Data;
using CondoVision.Data.Entities;

namespace CondoVision.Models.Interface
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IGenericRepository<Company> _companyRepository;

        public CompanyRepository(IGenericRepository<Company> companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await _companyRepository.GetAllAsync();
        }

        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            return await _companyRepository.GetByIdAsync(id);
        }

        public async Task<Company> AddCompanyAsync(Company company)
        {
            await _companyRepository.AddAsync(company);
            await _companyRepository.CompleteAsync(); 
            return company;
        }

        public async Task<Company> UpdateCompanyAsync(Company company)
        {
            await _companyRepository.UpdateAsync(company);
            await _companyRepository.CompleteAsync(); 
            return company;
        }

        public async Task DeleteCompanyAsync(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company != null)
            {
                await _companyRepository.UpdateAsync(company);
                await _companyRepository.CompleteAsync(); 
            }
        }
    }
}
