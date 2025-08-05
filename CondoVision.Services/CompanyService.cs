using CondoVision.Data;
using CondoVision.Data.Entities;

namespace CondoVision.Models.Interface
{
    public class CompanyService : ICompanyService
    {
        private readonly IGenericRepository<Company> _companyRepository;

        public CompanyService(IGenericRepository<Company> companyRepository)
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
            await _companyRepository.CompleteAsync(); // Salva as alterações
            return company;
        }

        public async Task<Company> UpdateCompanyAsync(Company company)
        {
            _companyRepository.Update(company);
            await _companyRepository.CompleteAsync(); // Salva as alterações
            return company;
        }

        public async Task DeleteCompanyAsync(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company != null)
            {
                _companyRepository.Delete(company);
                await _companyRepository.CompleteAsync(); // Salva as alterações
            }
        }
    }
}
