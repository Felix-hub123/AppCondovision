using CondoVision.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetNotificationsByCondominiumIdAsync(int condominiumId, int? companyId);
        Task<Notification> GetByIdAsync(int id, int? companyId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int? companyId);
    }
}
