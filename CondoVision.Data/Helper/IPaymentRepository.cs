using CondoVision.Models.Entities;

namespace CondoVision.Data.Helper
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(string userId);
        Task<IEnumerable<Payment>> GetOutstandingPaymentsAsync(string userId);
    }
}