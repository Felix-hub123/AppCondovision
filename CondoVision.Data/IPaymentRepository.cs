using CondoVision.Models.Entities;

namespace CondoVision.Data.Helper
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetPaymentsByUnitIdAsync(int unitId, int? companyId);
        Task<Payment> GetByIdAsync(int id, int? companyId);
        Task<IEnumerable<Payment>> GetUnpaidPaymentsAsync(int? companyId);
        Task<IEnumerable<Payment>> GetPaymentsByCreatorAsync(string createdById, int? companyId); 
        Task<IEnumerable<Payment>> GetPaymentsByValidatorAsync(string validatedById, int? companyId);
    }
}