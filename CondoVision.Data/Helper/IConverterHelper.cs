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

        User ToUser(RegisterUserViewModel model);
        EditProfileViewModel ToEditProfileViewModel(User user);
        User ToUser(EditProfileViewModel model, User existingUser);

        Condominium ToCondominium(CreateCondominiumViewModel model);

        CondominiumViewModel ToCondominiumViewModel(Condominium condominium);

       
        Condominium ToCondominium(CondominiumViewModel viewModel);

        List<UserListViewModel> ToUserListViewModel(List<User> users);


        IEnumerable<CondominiumViewModel> ToCondominiumViewModelList(IEnumerable<Condominium> condominiums);

        Task<EditCondominiumViewModel> ToEditCondominiumViewModelAsync(Condominium condominium);
        Company ToCompany(CompanyViewModel model);

        void UpdateCompany(Company company, CompanyViewModel model);

        CompanyViewModel ToCompanyViewModel(Company company);

        UnitViewModel ToViewModel(Unit unit);

        IEnumerable<UnitViewModel> ToViewModel(IEnumerable<Unit> units);

        Unit ToEntity(UnitViewModel model);

        void UpdateUnit(Unit unit, UnitViewModel model);
    }
}
