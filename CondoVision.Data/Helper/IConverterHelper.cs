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


        User ToUser(CreateUserViewModel model);

        User ToUser(CreateUserViewModel model, int companyId);

        List<UserListViewModel> ToUserListViewModelList(IEnumerable<User> users);
        

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

        EditUnitViewModel ToEditUnitViewModel(Unit unit);

        void UpdateUnit(Unit unit, EditUnitViewModel model);


        IEnumerable<UnitViewModel> ToViewModel(IEnumerable<Unit> units);

        Unit ToEntity(UnitViewModel model);

        void UpdateUnit(Unit unit, UnitViewModel model);

        FractionOwner ToFractionOwner(FractionOwnerViewModel viewModel);

        FractionOwnerViewModel ToFractionOwnerViewModel(FractionOwner fractionOwner);

        IEnumerable<FractionOwnerViewModel> ToFractionOwnerViewModelList(IEnumerable<FractionOwner> fractionOwners);

        List<CondominiumViewModel> ToCondominiumViewModelList(List<Condominium> condominiums);

        IEnumerable<CompanyViewModel> ToCompanyViewModelList(IEnumerable<Company> companies);

        void UpdateCondominium(Condominium condominium, EditCondominiumViewModel model);
    }
}
