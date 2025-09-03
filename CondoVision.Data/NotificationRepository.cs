using CondoVision.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(DataContext context) : base(context)
        {


        }

        public async Task<IEnumerable<Notification>> GetNotificationsByCondominiumIdAsync(int condominiumId, int? companyId)
        {
            return await _context.Notifications
                .Include(n => n.Condominium)
                .Where(n => n.CondominiumId == condominiumId && n.Condominium!.CompanyId == companyId && !n.WasDeleted)
                .ToListAsync();
        }

        public async Task<Notification> GetByIdAsync(int id, int? companyId)
        {
            return await _context.Notifications
                .Include(n => n.Condominium)
                .FirstOrDefaultAsync(n => n.Id == id && n.Condominium!.CompanyId == companyId && !n.WasDeleted);
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int? companyId)
        {
            return await _context.Notifications
                .Include(n => n.Condominium)
                .Where(n => !n.IsRead && n.Condominium!.CompanyId == companyId && !n.WasDeleted)
                .ToListAsync();
        }


    }
}
