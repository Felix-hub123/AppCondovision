using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CondoVision.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ILogger<SeedDb> _logger;

        public SeedDb(DataContext context, IUserHelper userHelper, ILogger<SeedDb> logger)
        {
            _context = context;
            _userHelper = userHelper;
            _logger = logger;
        }


        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckCompanyAsync();
            await CheckCondominiumAsync();
            await CheckAdminUserAsync();
        }

        private async Task CheckRolesAsync()
        {
            var roles = new[] { "CompanyAdmin", "CondoManager", "CondoOwner", "Employee" };
            foreach (var role in roles)
            {
                await _userHelper.CheckRoleAsync(role);
            }
            _logger.LogInformation("Roles verificadas/criadas com sucesso");
        }


        private async Task CheckCompanyAsync()
        {
            if (!_context.Companies.Any())
            {
                var company = new Company
                {
                    Name = "Gestão Exemplo Lda",
                    CompanyTaxId = "123456789",
                    Address = "Rua Exemplo, 123, Lisboa",
                    Contact = "geral@gestaoexemplo.pt",
                    CreationDate = DateTime.UtcNow,
                    WasDeleted = false
                };
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Empresa padrão criada com sucesso");
            }
        }


        private async Task CheckCondominiumAsync()
        {
            // Verificamos se já existem condomínios
            if (!_context.Condominiums.Any())
            {
                // Buscamos a empresa padrão para associar
                var company = await _context.Companies.FirstOrDefaultAsync();
                if (company == null)
                {
                    _logger.LogWarning("Nenhuma empresa encontrada para associar ao condomínio. O condomínio não será criado.");
                    return;
                }
            }
        }

        private async Task CheckAdminUserAsync()
        {
            var company = await _context.Companies.FirstOrDefaultAsync();
            if (company == null)
            {
                _logger.LogWarning("Nenhuma empresa encontrada para associar ao admin");
                return;
            }

            var adminEmail = "admin@condovision.pt";
            var user = await _userHelper.GetUserByEmailAsync(adminEmail);
            if (user == null)
            {
                user = new User
                {
                    FullName = "Administrador Sistema",
                    Email = adminEmail,
                    UserName = adminEmail,
                    TaxId = "123456789",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    Address = "Rua Admin, 1, Lisboa",
                    CompanyId = company.Id,
                    EmailConfirmed = true,
                    WasDeleted = false
                };

                var result = await _userHelper.AddUserAsync(user, "Admin123!");
                if (result.Succeeded)
                {
                    await _userHelper.AddUserToRoleAsync(user, "CompanyAdmin");
                    _logger.LogInformation("Usuário admin criado com sucesso");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError($"Erro ao criar usuário admin: {errors}");
                }
            }
        }
    }
}


    

