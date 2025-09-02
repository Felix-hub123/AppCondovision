using CondoVision.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data.Helper
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
       

        public PaymentRepository(DataContext context) : base(context)
        {
           
        }

        public async Task<Payment> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Where(p => !p.WasDeleted)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Where(p => !p.WasDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(Payment entity)
        {
            entity.CreationDate = DateTime.UtcNow;
            entity.WasDeleted = false;
            await _context.Payments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payment entity)
        {
            _context.Payments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Payment entity)
        {
            entity.WasDeleted = true;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<Payment, bool>> predicate)
        {
            return await _context.Payments
                .AnyAsync(predicate);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(string userId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Where(p => p.UserId == userId && !p.WasDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetOutstandingPaymentsAsync(string userId)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId && !p.WasDeleted && !p.IsPaid)
                .ToListAsync();
        }
    }
   
}
