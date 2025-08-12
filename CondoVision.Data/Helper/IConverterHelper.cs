using CondoVision.Data.Entities;
using CondoVision.Models;
using CondoVision.Models.Entities;
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

        User ToUser(RegisterNewUserViewModel model);
        EditProfileViewModel ToEditProfileViewModel(User user);
        User ToUser(EditProfileViewModel model, User existingUser);


        Fraction ToFraction(CreateFractionViewModel model);
        Fraction ToFraction(EditFractionViewModel model);
        FractionViewModel ToFractionViewModel(Fraction fraction);
        EditFractionViewModel ToEditFractionViewModel(Fraction fraction);

        FractionOwnerViewModel ToFractionOwnerViewModel(FractionOwner fractionOwner);
        FractionOwner ToFractionOwner(AssociateFractionOwnerViewModel model);
        FractionOwner ToFractionOwner(AssociateFractionOwnerViewModel model, FractionOwner existingFractionOwner);

    }
}
