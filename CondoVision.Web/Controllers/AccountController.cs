using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
        private readonly ICompanyRepository _companyRepository;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEMailHelper mailHelper,
            IUserRepository userRepository,
            IConverterHelper converterHelper,
            ICompanyRepository companyRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mailHelper = mailHelper;
            _userRepository = userRepository;
            _converterHelper = converterHelper;
            _companyRepository = companyRepository;
        }



        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? companyId, int page = 1, int pageSize = 10)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser?.CompanyId == null)
                return Forbid();

            companyId = companyId ?? currentUser.CompanyId;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;
            ViewData["EmailSortParm"] = string.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";

            var usersQuery = _userRepository.GetAllQueryable(companyId.Value)
                .Where(u => !u.WasDeleted);

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

            var model = new UserListViewModelWrapper
            {
                Items = _converterHelper.ToUserListViewModelList(users),
                TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize),
                CurrentPage = page
            };

            return View(model);
        }



        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !currentUser.CompanyId.HasValue)
            {
                return NotFound("Utilizador sem empresa associada.");
            }

            var model = new CreateUserViewModel { CompanyId = currentUser.CompanyId.Value };
            await PopulateViewBagsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !currentUser.CompanyId.HasValue)
                return NotFound("Utilizador sem empresa associada.");

            if (model.CompanyId != currentUser.CompanyId)
                ModelState.AddModelError(string.Empty, "Você só pode criar usuários para sua empresa.");

            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email!);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Este e-mail já está em uso.");
                    await PopulateViewBagsAsync(model);
                    return View(model);
                }

                var user = _converterHelper.ToUser(model);
                user.CompanyId = currentUser.CompanyId;

               
                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    var allowedRoles = new[] { "Condômino", "CondoManager", "CompanyAdmin" };
                    var role = !string.IsNullOrEmpty(model.RoleName) && allowedRoles.Contains(model.RoleName)
                        ? model.RoleName
                        : "Condômino";

                    await _userManager.AddToRoleAsync(user, role);

                  
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var setPasswordLink = Url.Action(
                        "ResetPassword",
                        "Account",
                        new { token, email = user.Email },
                        protocol: HttpContext.Request.Scheme
                    );

                    var emailMessage = $"Olá {user.FullName},<br/><br/>" +
                                       $"Você foi registrado no CondoVision. Para criar sua senha e acessar sua conta, clique no link abaixo:<br/>" +
                                       $"<a href='{setPasswordLink}'>Criar minha senha</a><br/><br/>" +
                                       $"Se não solicitou este cadastro, ignore este e-mail.";

                    await _mailHelper.SendEmailAsync(user.Email!, "Crie sua senha no CondoVision", emailMessage);

                    TempData["SuccessMessage"] = "Usuário criado com sucesso! Um e-mail foi enviado para definir a senha.";
                    return RedirectToAction(nameof(Index));
                }

                AddIdentityErrors(result);
            }

            await PopulateViewBagsAsync(model);
            return View(model);
        }


        private async Task PopulateViewBagsAsync(CreateUserViewModel model)
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                 new SelectListItem { Value = "Condômino", Text = "Condômino" },
                 new SelectListItem { Value = "CondoManager", Text = "Gestor de Condomínio" },
                 new SelectListItem { Value = "CompanyAdmin", Text = "Administrador de Empresa" }
            };

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !currentUser.CompanyId.HasValue)
            {
                ViewBag.Companies = new List<SelectListItem>
                {
                   new SelectListItem { Value = "", Text = "Sem empresas disponíveis" }
                };
                return;
            }

            try
            {
                var companies = await _companyRepository.GetCompaniesByCompanyIdAsync(currentUser.CompanyId.Value);
                var companyItems = companies
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();
                ViewBag.Companies = companyItems.Any() ? companyItems : new List<SelectListItem>
                {
                     new SelectListItem { Value = currentUser.CompanyId.ToString(), Text = "Sem empresas adicionais" }
                };
            }
            catch (Exception )
            {

                ViewBag.Companies = new List<SelectListItem>
                {
                     new SelectListItem { Value = currentUser.CompanyId.ToString(), Text = "Erro ao carregar empresas" }
                };
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return View("Error", new { Message = "Token ou ID de utilizador inválido." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error", new { Message = "Utilizador não encontrado." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View("ConfirmEmailSuccess");
            }
            return View("Error", new { Message = "Falha ao confirmar e-mail." });
        }

        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Edit(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser?.CompanyId == null)
                return Forbid();

            var user = await _userRepository.GetByIdAsync(id, currentUser.CompanyId);
            if (user == null)
                return NotFound();

            var model = _converterHelper.ToEditProfileViewModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Edit(string id, EditProfileViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser?.CompanyId == null)
                return Forbid();

            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetByIdAsync(id, currentUser.CompanyId);
                if (user != null)
                {
                    var updatedUser = _converterHelper.ToUser(model, user);
                    var result = await _userManager.UpdateAsync(updatedUser);
                    if (result.Succeeded)
                    {
                        await _userRepository.UpdateAsync(updatedUser);
                        return RedirectToAction(nameof(Index));
                    }
                    return View(model);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser?.CompanyId == null)
                return Forbid();

            var user = await _userRepository.GetByIdAsync(id, currentUser.CompanyId);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser?.CompanyId == null)
                return Forbid();

            var user = await _userRepository.GetByIdAsync(id, currentUser.CompanyId);
            if (user != null)
            {
                await _userRepository.DeleteAsync(user); // Usa soft delete
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

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

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Por favor, confirme seu e-mail antes de fazer login.");
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



        private string GenerateRandomPassword()
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> ManageUsers()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser?.CompanyId == null)
                return Forbid();

            var users = await _userRepository.GetAllAsync(currentUser.CompanyId);
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


        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("Utilizador não encontrado.");
                }

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword!, model.NewPassword!);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Senha alterada com sucesso!";
                    return View();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: ResetPasswordConfirmation
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        private void AddIdentityErrors(IdentityResult result)
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
