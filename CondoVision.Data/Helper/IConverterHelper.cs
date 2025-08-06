using CondoVision.Data.Entities;
using CondoVision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data.Helper
{
    public interface IConverterHelper
    {
        EditUserViewModel ToEditUserViewModel(User user, string roleName);
        User ToUser(CreateUserViewModel model, int companyId);
        User ToUser(EditUserViewModel model);

        User ToUser(EditUserViewModel model, User user);

    }
}
