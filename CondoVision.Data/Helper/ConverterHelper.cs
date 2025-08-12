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
                Id = model.Id!,
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

        /// <summary>
        /// Converte um RegisterNewUserViewModel para uma entidade User.
        /// </summary>
        public User ToUser(RegisterNewUserViewModel model)
        {
            return new User
            {
                // Mapeia as propriedades do ViewModel para a entidade User
                FullName = model.FullName,
                Email = model.Username,    // O Username do ViewModel é o Email do User
                UserName = model.Username, // O Username do ViewModel é o UserName do User
                PhoneNumber = model.PhoneNumber,

                // Outras propriedades com valores padrão para um novo registo
                TaxId = string.Empty,       // Pode ser preenchido mais tarde ou ser opcional no registo
                DateOfBirth = DateTime.MinValue, // Pode ser preenchido mais tarde ou ser opcional
                Address = string.Empty,     // Pode ser preenchido mais tarde ou ser opcional
                CompanyId = 0,              // Será associado por um administrador ou durante o seed
                EmailConfirmed = false,     // Por padrão, não confirmado até o utilizador clicar no link
                WasDeleted = false          // Por padrão, não eliminado
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

            // O Email (UserName) geralmente não é alterado através da edição de perfil simples,
            // pois está ligado à identidade e pode exigir um processo de validação separado.
            // existingUser.Email = model.Email; // Não descomentar a menos que tenha um fluxo para isso
            // existingUser.UserName = model.Email; // Não descomentar a menos que tenha um fluxo para isso

            return existingUser;
        }

        public Fraction ToFraction(CreateFractionViewModel model)
        {
            return new Fraction
            {
                Id = 0, 
                UnitNumber = model.UnitNumber,
                Floor = model.Floor,
                Block = model.Block,
                Area = model.Area,
                Permilage = model.Permilage,
                CondominiumId = model.CondominiumId,
                WasDeleted = false 
            };
        }

        public Fraction ToFraction(EditFractionViewModel model)
        {
            return new Fraction
            {
                Id = model.Id,
                UnitNumber = model.UnitNumber,
                Floor = model.Floor,
                Block = model.Block,
                Area = model.Area,
                Permilage = model.Permilage,
                CondominiumId = model.CondominiumId,
                WasDeleted = model.WasDeleted 
            };
        }

        public FractionViewModel ToFractionViewModel(Fraction fraction)
        {
            return new FractionViewModel
            {
                Id = fraction.Id,
                UnitNumber = fraction.UnitNumber,
                Floor = fraction.Floor,
                Block = fraction.Block,
                Area = fraction.Area,
                Permilage = fraction.Permilage,
                CondominiumId = fraction.CondominiumId,
                CondominiumName = fraction.Condominium?.Name, 
                WasDeleted = fraction.WasDeleted
            };
        }

        public EditFractionViewModel ToEditFractionViewModel(Fraction fraction)
        {
            return new EditFractionViewModel
            {
                Id = fraction.Id,
                UnitNumber = fraction.UnitNumber,
                Floor = fraction.Floor,
                Block = fraction.Block,
                Area = fraction.Area,
                Permilage = fraction.Permilage,
                CondominiumId = fraction.CondominiumId,
                WasDeleted = fraction.WasDeleted,
             
            };
        }


        public FractionOwnerViewModel ToFractionOwnerViewModel(FractionOwner fractionOwner)
        {
            return new FractionOwnerViewModel
            {
                Id = fractionOwner.Id,
                UnitNumber = fractionOwner.Fraction?.UnitNumber ?? "N/A",
                FractionFloor = fractionOwner.Fraction?.Floor,
                FractionBlock = fractionOwner.Fraction?.Block,
                CondominiumName = fractionOwner.Fraction?.Condominium?.Name ?? "N/A", 
                UserId = fractionOwner.UserId,
                OwnerFullName = fractionOwner.User?.FullName ?? "N/A", 
                OwnerEmail = fractionOwner.User?.Email ?? "N/A",
                WasDeleted = fractionOwner.WasDeleted
            };
        }

        public FractionOwner ToFractionOwner(AssociateFractionOwnerViewModel model)
        {
            return new FractionOwner
            {
                FractionId = model.FractionId,
                UserId = model.UserId ?? string.Empty,
                WasDeleted = false 
            };
        }


        public FractionOwner ToFractionOwner(AssociateFractionOwnerViewModel model, FractionOwner existingFractionOwner)
        {
           
            existingFractionOwner.FractionId = model.FractionId;
            existingFractionOwner.UserId = model.UserId ?? string.Empty;
            // existingFractionOwner.WasDeleted = model.WasDeleted; // WasDeleted deve ser gerido pela lógica de Delete

            return existingFractionOwner;
        }
    }
}
