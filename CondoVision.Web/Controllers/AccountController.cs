using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CondoVision.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEMailHelper _mailHelper;
        private readonly IUserRepository _userRepository;
        private readonly IConverterHelper _converterHelper;

        public AccountController(UserManager<User> userManager, 
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEMailHelper mailHelper,
            IUserRepository userRepository,
            IConverterHelper converterHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mailHelper = mailHelper;
            _userRepository = userRepository;
            _converterHelper = converterHelper;
        }



        public async Task<IActionResult> Index(string searchString, string sortOrder, int? companyId, int page = 1, int pageSize = 10)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !User.IsInRole("AdminCompany"))
                return Forbid(); 

            companyId = companyId ?? currentUser.CompanyId; 

            ViewData["CurrentFilter"] = searchString;
            ViewData["EmailSortParm"] = string.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";

            var usersQuery = _userRepository.GetAllQueryable().Where(u => u.CompanyId == companyId);

            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(u => u.Email!.Contains(searchString) || u.FullName!.Contains(searchString));
            }

            usersQuery = sortOrder switch
            {
                "email_desc" => usersQuery.OrderByDescending(u => u.Email),
                "name" => usersQuery.OrderBy(u => u.FullName),
                "name_desc" => usersQuery.OrderByDescending(u => u.FullName),
                _ => usersQuery.OrderBy(u => u.Email)
            };

            var totalUsers = await usersQuery.CountAsync();
            var users = usersQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = _converterHelper.ToUserListViewModel(users);
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
            ViewBag.CurrentPage = page;

            return View(model);
        }



        // GET: Register
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "As senhas não coincidem.");
                    return View(model);
                }

                var user = _converterHelper.ToUser(model);
                var result = await _userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role ?? "Condómino");
                    await _userRepository.AddAsync(user);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var link = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, protocol: HttpContext.Request.Scheme);
                    await _mailHelper.SendEmailAsync(user.Email!, "Confirmação de E-mail", $"Confirme seu e-mail <a href='{link}'>aqui</a>.");
                    return RedirectToAction("Login");
                }
                AddErrors(result);
            }
                   ViewBag.Roles = new List<SelectListItem>
                   {
                      new SelectListItem { Value = "AdminCompany", Text = "Administrador de Empresa" },
                      new SelectListItem { Value = "CondoManager", Text = "Gestor de Condomínio" },
                      new SelectListItem { Value = "Condómino", Text = "Condómino" }
                   };
                   return View(model);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var user = _converterHelper.ToUser(model);

                var result = await _userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "CondoOwner"); 
                    await _userRepository.AddAsync(user); 
                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
            }
            return View(model);
        }

      
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            
            var model = _converterHelper.ToEditProfileViewModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditProfileViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user != null)
                {
                    // Atualiza a entidade User existente com os dados do ViewModel
                    var updatedUser = _converterHelper.ToUser(model, user);

                    var result = await _userManager.UpdateAsync(updatedUser);
                    if (result.Succeeded)
                    {
                        await _userRepository.UpdateAsync(updatedUser); // Salva no repositório
                        return RedirectToAction(nameof(Index));
                    }
                    AddErrors(result);
                }
            }
            return View(model);
        }

        // Eliminar utilizador
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    ModelState.AddModelError(string.Empty, "O e-mail é obrigatório.");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError(string.Empty, "A senha é obrigatória.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            }
            return View(model);
        }

       

      

        // Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "AdminCompany")]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // GET: ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    ModelState.AddModelError(string.Empty, "O e-mail é obrigatório.");
                    return View(model);
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var link = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, protocol: HttpContext.Request.Scheme);
                    if (string.IsNullOrEmpty(link))
                    {
                        ModelState.AddModelError(string.Empty, "Falha ao gerar o link de redefinição.");
                        return View(model);
                    }
                    var body = $"<a href='{link}'>Clique aqui para redefinir sua senha</a>";
                    if (string.IsNullOrEmpty(user.Email))
                    {
                        ModelState.AddModelError(string.Empty, "E-mail do usuário não configurado.");
                        return View(model);
                    }
                    await _mailHelper.SendEmailAsync(user.Email, "Redefinição de Senha", body);
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                ModelState.AddModelError(string.Empty, "E-mail não encontrado ou não confirmado.");
            }
            return View(model);
        }

        // GET: ForgotPasswordConfirmation
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: ResetPassword
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Token = token, NewPassword = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.NewPassword))
                {
                    ModelState.AddModelError(string.Empty, "O e-mail é obrigatório.");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.Token))
                {
                    ModelState.AddModelError(string.Empty, "O token é obrigatório.");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.NewPassword))
                {
                    ModelState.AddModelError(string.Empty, "A nova senha é obrigatória.");
                    return View(model);
                }

                var user = await _userManager.FindByEmailAsync(model.NewPassword);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ResetPasswordConfirmation");
                    }
                    AddErrors(result);
                }
                ModelState.AddModelError(string.Empty, "E-mail ou token inválido.");
            }
            return View(model);
        }

        // GET: ResetPasswordConfirmation
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            var model = new ManageRolesViewModel
            {
                UserId = userId,
                Roles = roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> ManageRoles(ManageRolesViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserId))
            {
                return BadRequest("O ID do usuário é obrigatório.");
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var selectedRole = model.RoleName;

            if (currentRoles.Any() && !string.IsNullOrEmpty(selectedRole) && !currentRoles.Contains(selectedRole))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            if (!string.IsNullOrEmpty(selectedRole) && !currentRoles.Contains(selectedRole))
            {
                await _userManager.AddToRoleAsync(user, selectedRole);
            }

            return RedirectToAction("ManageUsers");
        }


        [Authorize]
        public async Task<IActionResult> Enable2FA()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("Usuário não autenticado.");
            }

            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                ModelState.AddModelError(string.Empty, "Por favor, configure seu número de telefone antes de ativar o 2FA.");
                return View(new Enable2FAViewModel());
            }

            var authenticatorKey = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");
            var qrCodeUrl = $"otpauth://totp/CondoVision:{user.Email}?secret={authenticatorKey}&issuer=CondoVision";
            var model = new Enable2FAViewModel { QRCodeImageUrl = qrCodeUrl };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Enable2FA(Enable2FAViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("Usuário não autenticado.");
            }

            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                ModelState.AddModelError(string.Empty, "Número de telefone não configurado.");
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Code))
            {
                ModelState.AddModelError(string.Empty, "O código de verificação é obrigatório.");
                return View(model);
            }

            var result = await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", model.Code);
            if (result)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Código de verificação inválido.");
            return View(model);
        }


        [HttpPost("api/register")]
        public async Task<IActionResult> ApiRegister(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _converterHelper.ToUser(model);
                var result = await _userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role ?? "Condómino");
                    await _userRepository.AddAsync(user);
                    return Ok(new { Message = "Usuário criado com sucesso." });
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("api/login")]
        public async Task<IActionResult> ApiLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email!, model.Password!, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return Ok(new { Message = "Login bem-sucedido." });
                }
                return BadRequest("Login falhou.");
            }
            return BadRequest(ModelState);
        }

    }
}
