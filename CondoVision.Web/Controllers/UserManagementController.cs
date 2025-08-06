using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models;
using CondoVision.Models.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CondoVision.Web.Controllers
{

    [Authorize(Roles = "CompanyAdmin")]
    public class UserManagementController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly UserManager<User> _userManager;
        private readonly ICompanyRepository _companyRepository;
        private readonly IConverterHelper _converterHelper;
        RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            IUserHelper userHelper,
            UserManager<User> userManager,
            ICompanyRepository companyRepository,
            IConverterHelper converterHelper,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserManagementController> logger)
        {
            _userHelper = userHelper;
            _userManager = userManager;
            _roleManager = roleManager;
            _companyRepository = companyRepository;
            _converterHelper = converterHelper;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.Include(u => u.Company).ToListAsync();
            return View(users);
        }


        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userHelper.GetUserWithDetailsAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Companies = new SelectList(await _companyRepository.GetAllCompaniesAsync(), "Id", "Name");
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Note: Esta lógica é melhor na UserHelper para evitar duplicação.
                var companyId = model.CompanyId;
                var user = _converterHelper.ToUser(model, companyId);

                var result = await _userHelper.AddUserAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userHelper.AddUserToRoleAsync(user, model.RoleName);
                    _logger.LogInformation($"Usuário '{user.FullName}' criado com sucesso.");
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Companies = new SelectList(await _companyRepository.GetAllCompaniesAsync(), "Id", "Name", model.CompanyId);
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name", model.RoleName);
            return View(model);
        }


        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userHelper.GetUserWithDetailsAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _userHelper.GetUserRolesAsync(user);
            var currentRole = roles.FirstOrDefault() ?? string.Empty; 
            var viewModel = _converterHelper.ToEditUserViewModel(user, currentRole);

            ViewBag.Companies = new SelectList(await _companyRepository.GetAllCompaniesAsync(), "Id", "Name", viewModel.CompanyId);
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name", viewModel.RoleName);

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

               
                _converterHelper.ToUser(model, user);

                var result = await _userHelper.UpdateUserAsync(user);

                if (result.Succeeded)
                {
                    var currentRoles = await _userHelper.GetUserRolesAsync(user);
                    var newRoleName = model.RoleName ?? string.Empty; 

                    if (currentRoles.FirstOrDefault() != newRoleName)
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        await _userHelper.AddUserToRoleAsync(user, newRoleName);
                    }
                    _logger.LogInformation($"Usuário '{user.FullName}' atualizado com sucesso.");
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Companies = new SelectList(await _companyRepository.GetAllCompaniesAsync(), "Id", "Name", model.CompanyId);
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name", model.RoleName);
            return View(model);
        }


        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userHelper.GetUserWithDetailsAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userHelper.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userHelper.DeleteUserAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Usuário '{user.FullName}' eliminado (soft-delete) com sucesso.");
                return RedirectToAction(nameof(Index));
            }
            _logger.LogError($"Erro ao eliminar o usuário '{user.FullName}'.");
            return RedirectToAction(nameof(Index));
        }

    }
}
