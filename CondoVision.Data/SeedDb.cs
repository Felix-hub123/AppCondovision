using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models.Entities;
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
        private readonly UserManager<User> _userManager;

        public SeedDb(
            DataContext context,
            IUserHelper userHelper,
            ILogger<SeedDb> logger,
            RoleManager<IdentityRole> roleManager,
             UserManager<User> userManager)
        {
            _context = context;
            _userHelper = userHelper;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public async Task SeedAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
                await CheckRolesAsync();
                await CheckCompanyAsync();
                await CheckCondominiumAsync();
                await SeedUsersAsync();
                await SeedCondominiumUsersAndUnitsAsync();
                _logger.LogInformation("Seeding concluído com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o processo de seeding.");
                throw;
            }
        }

        private async Task CheckRolesAsync()
        {
            var roles = new[] { "CompanyAdmin", "CondoManager", "CondoOwner", "Employee", "Condómino" };
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
                    City = "Porto",
                    PostalCode = "4000-002",
                    CompanyId = company.Id,
                    RegistrationDate = DateTime.UtcNow,
                    WasDeleted = false
                };
                _context.Condominiums.Add(condominium);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Condomínio padrão criado com sucesso.");
            }
        }

        private async Task<User?> EnsureUserWithRoleAsync(string email, string password, string roleName, string fullName, bool emailConfirmed, bool wasDeleted, string phoneNumber)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FullName = fullName,
                    UserType = roleName, 
                    Email = email,
                    UserName = email,
                    EmailConfirmed = emailConfirmed,
                    WasDeleted = wasDeleted,
                    PhoneNumber = phoneNumber
                };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
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
                if (!await _userManager.IsInRoleAsync(user, roleName))
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                    _logger.LogInformation($"Utilizador '{fullName}' ({email}) adicionado à role '{roleName}'.");
                }
                if (string.IsNullOrEmpty(user.PhoneNumber))
                {
                    user.PhoneNumber = phoneNumber;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Número de telefone '{phoneNumber}' adicionado ao utilizador '{fullName}' ({email}).");
                }
                return user;
            }
        }

        private async Task SeedUsersAsync()
        {
            await EnsureUserWithRoleAsync("admin@condovision.pt", "Admin123!", "CompanyAdmin", "Administrador Sistema", true, false, "+351912345678");
            await EnsureUserWithRoleAsync("condomanager@condovision.pt", "Manager123!", "CondoManager", "Gestor Condomínio", true, false, "+351923456789");
            await EnsureUserWithRoleAsync("condoowner@condovision.pt", "Owner123!", "CondoOwner", "Proprietário Condomínio", true, false, "+351934567890");
            await EnsureUserWithRoleAsync("employee@condovision.pt", "Employee123!", "Employee", "Colaborador Empresa", true, false, "+351945678901");
            await EnsureUserWithRoleAsync("condomino@condovision.pt", "Condo123!", "Condómino", "Residente Condomínio", true, false, "+351956789012");
            await EnsureUserWithRoleAsync("testuser@condovision.pt", "Test123!", "Condómino", "Usuário Teste", true, false, "+351967890123");
        }

        private async Task SeedCondominiumUsersAndUnitsAsync()
        {
            var company = await _context.Companies.FirstOrDefaultAsync();
            if (company == null)
            {
                _logger.LogWarning("Nenhuma empresa encontrada. Não será possível associar gestores ou unidades.");
                return;
            }

            var condominiums = await _context.Condominiums.ToListAsync();
            if (!condominiums.Any())
            {
                _logger.LogWarning("Nenhum condomínio encontrado. Não será possível associar gestores ou unidades.");
                return;
            }

            var manager = await _userManager.FindByEmailAsync("condomanager@condovision.pt");
            var owner = await _userManager.FindByEmailAsync("condoowner@condovision.pt");
            var condomini = await _userManager.FindByEmailAsync("condomino@condovision.pt");

            if (manager != null && owner != null && condomini != null)
            {
                
                if (!await _context.CondominiumUsers.AnyAsync(cu => cu.CondominiumId == condominiums[0].Id && cu.UserId == manager.Id))
                {
                    var condoManager = new CondominiumUser
                    {
                        CondominiumId = condominiums[0].Id,
                        UserId = manager.Id,
                        WasDeleted = false
                    };
                    _context.CondominiumUsers.Add(condoManager);
                }
                else
                {
                    _logger.LogInformation($"Associação entre Condomínio {condominiums[0].Id} e Gestor {manager.Id} já existe.");
                }

                
                if (!await _context.CondominiumUsers.AnyAsync(cu => cu.CondominiumId == condominiums[0].Id && cu.UserId == condomini.Id))
                {
                    var condoUser = new CondominiumUser
                    {
                        CondominiumId = condominiums[0].Id,
                        UserId = condomini.Id,
                        WasDeleted = false
                    };
                    _context.CondominiumUsers.Add(condoUser);
                }
                else
                {
                    _logger.LogInformation($"Associação entre Condomínio {condominiums[0].Id} e Condómino {condomini.Id} já existe.");
                }

              
                var unitAExists = await _context.Units.AnyAsync(u => u.CondominiumId == condominiums[0].Id && u.UnitName == "A" && !u.WasDeleted);
                var unitBExists = await _context.Units.AnyAsync(u => u.CondominiumId == condominiums[0].Id && u.UnitName == "B" && !u.WasDeleted);

                if (!unitAExists || !unitBExists)
                {
                    if (!unitAExists)
                    {
                        var unit1 = new Unit
                        {
                            UnitName = "A",
                            Permillage = 50.00m,
                            CondominiumId = condominiums[0].Id,
                            OwnerId = owner.Id,
                            WasDeleted = false
                        };
                        _context.Units.Add(unit1);
                        _logger.LogInformation("Unidade A adicionada ao Condomínio {CondominiumId}.", condominiums[0].Id);
                    }
                    if (!unitBExists)
                    {
                        var unit2 = new Unit
                        {
                            UnitName = "B",
                            Permillage = 50.00m,
                            CondominiumId = condominiums[0].Id,
                            OwnerId = owner.Id,
                            WasDeleted = false
                        };
                        _context.Units.Add(unit2);
                        _logger.LogInformation("Unidade B adicionada ao Condomínio {CondominiumId}.", condominiums[0].Id);
                    }
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Unidades A e/ou B adicionadas ao Condomínio {CondominiumId} com sucesso.", condominiums[0].Id);
                }
                else
                {
                    _logger.LogInformation("Unidades A e B já existem para o Condomínio {CondominiumId}.", condominiums[0].Id);
                }
            }
        }
    }
}

    









