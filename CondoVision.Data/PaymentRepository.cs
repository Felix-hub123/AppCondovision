using CondoVision.Data.Helper;
using CondoVision.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
       

        public PaymentRepository(DataContext context) : base(context)
        {
           
        }


        public async Task<IEnumerable<Payment>> GetPaymentsByUnitIdAsync(int unitId, int? companyId)
        {
            return await _context.Payments
                .Include(p => p.Unit)
                .Include(p => p.CreatedBy)
                .Include(p => p.ValidatedBy)
                .Where(p => p.UnitId == unitId && p.Unit.CompanyId == companyId && !p.WasDeleted)
                .ToListAsync();
        }

        public async Task<Payment> GetByIdAsync(int id, int? companyId)
        {
            return await _context.Payments
                .Include(p => p.Unit)
                .Include(p => p.CreatedBy)
                .Include(p => p.ValidatedBy)
                .FirstOrDefaultAsync(p => p.Id == id && p.Unit.CompanyId == companyId && !p.WasDeleted);
        }

        public async Task<IEnumerable<Payment>> GetUnpaidPaymentsAsync(int? companyId)
        {
            return await _context.Payments
                .Include(p => p.Unit)
                .Where(p => !p.IsPaid && p.Unit!.CompanyId == companyId && !p.WasDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByCreatorAsync(string createdById, int? companyId)
        {
            return await _context.Payments
                .Include(p => p.Unit)
                .Where(p => p.CreatedById == createdById && p.Unit.CompanyId == companyId && !p.WasDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByValidatorAsync(string validatedById, int? companyId)
        {
            return await _context.Payments
                .Include(p => p.Unit)
                .Where(p => p.ValidatedById == validatedById && p.Unit.CompanyId == companyId && !p.WasDeleted)
                .ToListAsync();
        }

    }
   
}
