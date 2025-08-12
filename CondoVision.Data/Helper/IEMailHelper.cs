using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data.Helper
{
    public interface IEMailHelper
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}
