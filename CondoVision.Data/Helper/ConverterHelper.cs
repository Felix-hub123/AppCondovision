using CondoVision.Data.Entities;
using CondoVision.Models;
using CondoVision.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data.Helper
{
    public class ConverterHelper : IConverterHelper
    {

        private readonly ICompanyRepository _companyRepository;

        public ConverterHelper(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }
        /// <summary>
        /// Converte um User para EditUserViewModel.
        /// </summary>
        public EditUserViewModel ToEditUserViewModel(User user, string roleName)
        {
            return new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = roleName
            };
        }

        /// <summary>
        /// Converte um CreateUserViewModel para User.
        /// </summary>
        public User ToUser(CreateUserViewModel model, int companyId)
        {
            return new User
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
                WasDeleted = false,
                CompanyId = companyId, 
                TaxId = model.TaxId,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address
            };
        }


        public User ToUser(EditUserViewModel model, User user)
        {
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email; 
            return user;
        }
        public User ToUser(EditUserViewModel model)
        {
            return new User
            {
                Id = model.Id!,
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
                WasDeleted = false 
            };
        }

        /// <summary>
        /// Converte um CompanyViewModel para Company.
        /// </summary>
        public Company ToCompany(CompanyViewModel model)
        {
            return new Company
            {
                Id = model.Id,
                Name = model.Name,
                CompanyTaxId = model.CompanyTaxId,
                Address = model.Address,
                Contact = model.Contact,
                LogoId = model.LogoId,
                CreationDate = model.CreationDate,
                WasDeleted = model.WasDeleted
            };
        }

        /// <summary>
        /// Converte um Company para CompanyViewModel.
        /// </summary>
        public CompanyViewModel ToCompanyViewModel(Company company)
        {
            return new CompanyViewModel
            {
                Id = company.Id,
                Name = company.Name,
                CompanyTaxId = company.CompanyTaxId,
                Address = company.Address,
                Contact = company.Contact,
                LogoId = company.LogoId,
                CreationDate = company.CreationDate,
                WasDeleted = company.WasDeleted
            };
        }

        /// <summary>
        /// Atualiza um Company existente com dados de CompanyViewModel.
        /// </summary>
        public void UpdateCompany(Company company, CompanyViewModel model)
        {
            company.Name = model.Name;
            company.CompanyTaxId = model.CompanyTaxId;
            company.Address = model.Address;
            company.Contact = model.Contact;
            company.LogoId = model.LogoId;
            company.CreationDate = model.CreationDate; // Pode ser mantido original no controller
            company.WasDeleted = model.WasDeleted;
        }

        /// <summary>
        /// Converte um RegisterUserViewModel para uma entidade User.
        /// </summary>
        public User ToUser(RegisterUserViewModel model)
        {
            return new User
            {
                FullName = model.FullName,
                Email = model.Username,
                UserName = model.Username,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = false,
                WasDeleted = false
            };
        }

        /// <summary>
        /// Converte uma entidade User para um EditProfileViewModel.
        /// </summary>
        public EditProfileViewModel ToEditProfileViewModel(User user)
        {
            return new EditProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email, // O Email do User é usado no ViewModel
                PhoneNumber = user.PhoneNumber,
                ImageId = user.ImageId // Mapeia o ID da imagem
            };
        }

        /// <summary>
        /// Atualiza uma entidade User existente com dados de um EditProfileViewModel.
        /// </summary>
        public User ToUser(EditProfileViewModel model, User existingUser)
        {
            // Atualiza as propriedades do utilizador existente com base no ViewModel
            existingUser.FullName = model.FullName;
            existingUser.PhoneNumber = model.PhoneNumber;
            existingUser.ImageId = model.ImageId;

            return existingUser;
        }

      

        public CondominiumViewModel ToCondominiumViewModel(Condominium condominium)
        {
            return new CondominiumViewModel
            {
                Id = condominium.Id,
                Name = condominium.Name,
                Address = condominium.Address,
                City = condominium.City,
                PostalCode = condominium.PostalCode,
                CompanyId = condominium.CompanyId,
                RegistrationDate = condominium.RegistrationDate,
                WasDeleted = condominium.WasDeleted
            };
        }

        /// <summary>
        /// Converte um CreateCondominiumViewModel para uma entidade Condominium.
        /// </summary>
        public Condominium ToCondominium(CondominiumViewModel viewModel)
        {
            return new Condominium
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Address = viewModel.Address,
                City = viewModel.City,
                PostalCode = viewModel.PostalCode,
                CompanyId = viewModel.CompanyId,
                RegistrationDate = viewModel.RegistrationDate,
                WasDeleted = viewModel.WasDeleted
            };
        }


        public Condominium ToCondominium(CreateCondominiumViewModel model)
        {
            return new Condominium
            {
                Name = model.Name,
                Address = model.Address,
                City = model.City,
                PostalCode = model.PostalCode,
                CompanyId = model.CompanyId,
                RegistrationDate = DateTime.UtcNow,
                WasDeleted = false
            };
        }

        public IEnumerable<CondominiumViewModel> ToCondominiumViewModelList(IEnumerable<Condominium> condominiums)
        {
            return condominiums.Select(c => ToCondominiumViewModel(c)).ToList();
        }

        /// <summary>
        /// Atualiza uma entidade Condominium existente com dados de um EditCondominiumViewModel.
        /// </summary>


        public async Task<EditCondominiumViewModel> ToEditCondominiumViewModelAsync(Condominium condominium)
        {
            var model = new EditCondominiumViewModel
            {
                Id = condominium.Id,
                Name = condominium.Name,
                Address = condominium.Address,
                City = condominium.City,
                PostalCode = condominium.PostalCode,
                CompanyId = condominium.CompanyId,
                RegistrationDate = condominium.RegistrationDate,
                WasDeleted = condominium.WasDeleted,
                Companies = await GetCompaniesSelectList()
            };
            return model;
        }

        public List<UserListViewModel> ToUserListViewModel(List<User> users)
        {
            return users.Select(u => new UserListViewModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber
            }).ToList();
        }

        private async Task<IEnumerable<SelectListItem>> GetCompaniesSelectList()
        {
            return (await _companyRepository.GetAllCompaniesAsync())
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
        }

        public UnitViewModel ToViewModel(Unit unit)
        {
            if (unit == null)
                return null!;
            return new UnitViewModel
            {
                Id = unit.Id,
                UnitName = unit.UnitName,
                Permillage = unit.Permillage,
                OwnerId = unit.OwnerId,
                OwnerFullName = unit.Owner?.FullName,
                CondominiumId = unit.CondominiumId,
                CondominiumName = unit.Condominium?.Name
            };
        }

        public IEnumerable<UnitViewModel> ToViewModel(IEnumerable<Unit> units)
        {
            return units?.Select(ToViewModel) ?? Enumerable.Empty<UnitViewModel>();
        }

        public Unit ToEntity(UnitViewModel model)
        {
            if (model == null)
                return null!;
            return new Unit
            {
                Id = model.Id ?? 0,
                UnitName = model.UnitName,
                Permillage = model.Permillage,
                OwnerId = model.OwnerId,
                CondominiumId = model.CondominiumId
            };
        }

        public void UpdateUnit(Unit unit, UnitViewModel model)
        {
            if (unit == null || model == null)
                return;

            unit.UnitName = model.UnitName;
            unit.Permillage = model.Permillage;
            unit.OwnerId = model.OwnerId;
            unit.CondominiumId = model.CondominiumId; 
        }
    }


}

