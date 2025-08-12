using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CondoVision.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ILogger<SeedDb> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedDb(
            DataContext context,
            IUserHelper userHelper,
            ILogger<SeedDb> logger,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userHelper = userHelper;
            _logger = logger;
            _roleManager = roleManager;
        }


        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckCompanyAsync();
            await CheckCondominiumAsync();
            await SeedUsersAsync();
        }

        private async Task CheckRolesAsync()
        {
            var roles = new[] { "CompanyAdmin", "CondoManager", "CondoOwner", "Employee" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation($"Role '{role}' criada com sucesso.");
                }
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
            if (!_context.Condominiums.Any())
            {
                var company = await _context.Companies.FirstOrDefaultAsync();
                if (company == null)
                {
                    _logger.LogWarning("Nenhuma empresa encontrada para associar ao condomínio. O condomínio não será criado.");
                    return;
                }

                var condominium = new Condominium
                {
                    Name = "Condomínio Central",
                    Address = "Avenida Principal, 456, Porto",
                    CompanyId = company.Id,
                    RegistrationDate = DateTime.UtcNow,
                    WasDeleted = false
                };
                _context.Condominiums.Add(condominium);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Condomínio padrão criado com sucesso.");
            }
        }

        /// <summary>
        /// Método genérico para garantir que um utilizador existe e tem a role especificada.
        /// </summary>
        private async Task<User?> EnsureUserWithRoleAsync(string email, string password, string roleName, string fullName, string taxId, DateTime dateOfBirth, string address, bool emailConfirmed, bool wasDeleted)
        {
            var company = await _context.Companies.FirstOrDefaultAsync();
            if (company == null)
            {
                _logger.LogWarning($"Nenhuma empresa encontrada para associar ao utilizador {email}. O utilizador não será criado.");
                return null;
            }

            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FullName = fullName,
                    Email = email,
                    UserName = email,
                    TaxId = taxId,
                    DateOfBirth = dateOfBirth,
                    Address = address,
                    CompanyId = company.Id,
                    EmailConfirmed = emailConfirmed,
                    WasDeleted = wasDeleted
                };

                var result = await _userHelper.AddUserAsync(user, password);
                if (result.Succeeded)
                {
                    await _userHelper.AddUserToRoleAsync(user, roleName);
                    _logger.LogInformation($"Utilizador '{fullName}' ({email}) com role '{roleName}' criado com sucesso.");
                    return user;
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError($"Erro ao criar utilizador '{fullName}' ({email}): {errors}");
                    return null;
                }
            }
            else
            {
                _logger.LogInformation($"Utilizador '{fullName}' ({email}) já existe. Verificando role...");
                if (!await _userHelper.IsUserInRoleAsync(user, roleName))
                {
                    await _userHelper.AddUserToRoleAsync(user, roleName);
                    _logger.LogInformation($"Utilizador '{fullName}' ({email}) adicionado à role '{roleName}'.");
                }
                return user;
            }
        }


        /// <summary>
        /// Método genérico para garantir que um utilizador existe e tem a role especificada.
        /// </summary>

        private async Task SeedUsersAsync()
        {
            // Utilizadores de exemplo com as roles e dados definidos no PDF
            await EnsureUserWithRoleAsync("admin@condovision.pt", "Admin123!", "CompanyAdmin", "Administrador Sistema", "123456789", new DateTime(1980, 1, 1), "Rua Admin, 1, Lisboa", true, false);
            await EnsureUserWithRoleAsync("condomanager@condovision.pt", "Manager123!", "CondoManager", "Gestor Condomínio", "987654321", new DateTime(1985, 5, 10), "Rua do Gestor, 10, Porto", true, false);
            await EnsureUserWithRoleAsync("condoowner@condovision.pt", "Owner123!", "CondoOwner", "Proprietário Condomínio", "112233445", new DateTime(1970, 11, 20), "Rua do Proprietário, 5, Lisboa", true, false);
            await EnsureUserWithRoleAsync("employee@condovision.pt", "Employee123!", "Employee", "Colaborador Empresa", "554433221", new DateTime(1992, 3, 15), "Rua do Colaborador, 22, Coimbra", true, false);
        }
    }


}





