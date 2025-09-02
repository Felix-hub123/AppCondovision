using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
public class SeedDb
{
    private readonly DataContext _context;
    private readonly IUserHelper _userHelper;
    private readonly ILogger<SeedDb> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;

    public SeedDb(DataContext context, IUserHelper userHelper, ILogger<SeedDb> logger, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
    {
        _context = context;
        _userHelper = userHelper;
        _logger = logger;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Database.MigrateAsync();
            await CheckRolesAsync();
            await CheckCompanyAsync(); 
            await SeedUsersAsync();
            await CheckCondominiumAsync();
            await SeedCondominiumUsersAndUnitsAsync();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Seeding concluído com sucesso.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
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
            var admin = await _userManager.FindByEmailAsync("admin@condovision.pt");
            if (admin == null)
            {
                _logger.LogWarning("Usuário admin não encontrado para associar como criador da empresa.");
                return;
            }

            var company = new Company
            {
                Name = "Gestão Exemplo Lda",
                CompanyTaxId = "123456789",
                CreationDate = DateTime.UtcNow,
                WasDeleted = false,
                CreatedById = admin.Id
            };
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Empresa padrão criada com sucesso");

          
            var companyId = company.Id;
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                if (user.CompanyId == null || user.CompanyId == 0)
                {
                    user.CompanyId = companyId;
                }
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Usuários atualizados com CompanyId.");
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
        if (user != null && user.WasDeleted)
        {
            _logger.LogWarning($"Usuário '{email}' marcado como deletado. Removendo para recriar.");
            await _userManager.DeleteAsync(user);
            user = null;
        }

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
                PhoneNumber = phoneNumber,
                CompanyId = null 
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
                _logger.LogError($"Erro ao criar utilizador '{fullName}' ({email}): {string.Join(", ", result.Errors.Select(e => e.Description))}");
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
        await EnsureUserWithRoleAsync("admin@condovision.pt", "Admin123!Strong2025", "CompanyAdmin", "Administrador Sistema", true, false, "+351912345678");
        var admin = await _userManager.FindByEmailAsync("admin@condovision.pt");
        if (admin == null)
        {
            _logger.LogError("Falha ao criar ou encontrar o usuário admin@condovision.pt.");
            return;
        }
        _logger.LogInformation($"Usuário admin criado com ID: {admin.Id}");
        await EnsureUserWithRoleAsync("condomanager@condovision.pt", "Manager123!Strong2025", "CondoManager", "Gestor Condomínio", true, false, "+351923456789");
        await EnsureUserWithRoleAsync("condoowner@condovision.pt", "Owner123!Strong2025", "CondoOwner", "Proprietário Condomínio", true, false, "+351934567890");
        await EnsureUserWithRoleAsync("employee@condovision.pt", "Employee123!Strong2025", "Employee", "Colaborador Empresa", true, false, "+351945678901");
        await EnsureUserWithRoleAsync("condomino@condovision.pt", "Condo123!Strong2025", "Condómino", "Residente Condomínio", true, false, "+351956789012");
        await EnsureUserWithRoleAsync("testuser@condovision.pt", "Test123!Strong2025", "Condómino", "Usuário Teste", true, false, "+351967890123");
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
                _context.CondominiumUsers.Add(new CondominiumUser { CondominiumId = condominiums[0].Id, UserId = manager.Id, WasDeleted = false });
            }
            if (!await _context.CondominiumUsers.AnyAsync(cu => cu.CondominiumId == condominiums[0].Id && cu.UserId == condomini.Id))
            {
                _context.CondominiumUsers.Add(new CondominiumUser { CondominiumId = condominiums[0].Id, UserId = condomini.Id, WasDeleted = false });
            }

            var unitNames = new[] { "A", "B" };
            foreach (var unitName in unitNames)
            {
                if (!await _context.Units.AnyAsync(u => u.CondominiumId == condominiums[0].Id && u.UnitName == unitName && !u.WasDeleted))
                {
                    _context.Units.Add(new Unit { CondominiumId = condominiums[0].Id, UnitName = unitName, WasDeleted = false });
                }
            }

            var unitA = await _context.Units.FirstOrDefaultAsync(u => u.CondominiumId == condominiums[0].Id && u.UnitName == "A" && !u.WasDeleted);
            var unitB = await _context.Units.FirstOrDefaultAsync(u => u.CondominiumId == condominiums[0].Id && u.UnitName == "B" && !u.WasDeleted);

            if (unitA != null && unitB != null)
            {
                if (!await _context.FractionOwners.AnyAsync(fo => fo.UnitId == unitA.Id && !fo.WasDeleted))
                {
                    _context.FractionOwners.Add(new FractionOwner
                    {
                        UnitId = unitA.Id,
                        UserId = owner.Id,
                        UnitNumber = unitA.UnitName!,
                        FractionFloor = "1",
                        FractionBlock = "A",
                        OwnerFullName = owner.FullName ?? owner.UserName!,
                        OwnerEmail = owner.Email ?? "sem-email@condovision.pt",
                        WasDeleted = false
                    });
                }
                if (!await _context.FractionOwners.AnyAsync(fo => fo.UnitId == unitB.Id && !fo.WasDeleted))
                {
                    _context.FractionOwners.Add(new FractionOwner
                    {
                        UnitId = unitB.Id,
                        UserId = condomini.Id,
                        UnitNumber = unitB.UnitName!,
                        FractionFloor = "2",
                        FractionBlock = "B",
                        OwnerFullName = condomini.FullName ?? condomini.UserName!,
                        OwnerEmail = condomini.Email ?? "sem-email@condovision.pt",
                        WasDeleted = false
                    });
                }
            }

            if (!await _context.RecentActivities.AnyAsync())
            {
                var activities = new List<RecentActivity>
                {
                    new RecentActivity { UserName = manager.UserName, Action = "Criou nova Empresa", Date = DateTime.Now.AddDays(-14) },
                    new RecentActivity { UserName = owner.UserName, Action = "Atualizou Condomínio", Date = DateTime.Now.AddDays(-7) },
                    new RecentActivity { UserName = condomini.UserName, Action = "Eliminou Fração", Date = DateTime.Now.AddDays(-1) }
                };
                _context.RecentActivities.AddRange(activities);
            }
        }
    }
}










