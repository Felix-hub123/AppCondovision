using CondoVision.Data.Entities;
using CondoVision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data.Helper
{
    public class ConverterHelper : IConverterHelper
    {
        public EditUserViewModel ToEditUserViewModel(User user, string roleName)
        {
            return new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                TaxId = user.TaxId,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                CompanyId = user.CompanyId,
                RoleName = roleName
            };
        }

        public User ToUser(CreateUserViewModel model, int companyId)
        {
            return new User
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
                TaxId = model.TaxId,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                CompanyId = companyId,
                WasDeleted = false
            };
        }


        public User ToUser(EditUserViewModel model, User user)
        {
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email; 
            user.TaxId = model.TaxId;
            user.Address = model.Address;
            user.DateOfBirth = model.DateOfBirth;
            user.CompanyId = model.CompanyId;

            return user;
        }
        public User ToUser(EditUserViewModel model)
        {
            return new User
            {
                Id = model.Id,
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
                TaxId = model.TaxId,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                CompanyId = model.CompanyId,
                WasDeleted = false 
            };
        }
    }
}
