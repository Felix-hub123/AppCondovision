using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CondoVision.Web.Controllers
{
    public class AccountController : Controller
    {

        private readonly IUserHelper _userHelper;
        private readonly IEMailHelper _mailHelper;
        private readonly IConfiguration _configuration;
        private readonly IBlobHelper _blobHelper;

        public AccountController(
            IUserHelper userHelper,
            IEMailHelper mailHelper,
            IConfiguration configuration,
            IBlobHelper blobHelper)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
            _blobHelper = blobHelper;
        }

        // GET: Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userHelper.LoginAsync(model);

            if (result.Succeeded)
            {
                if (this.Request.Query.Keys.Contains("ReturnUrl"))
                {
        
                    var returnUrl = this.Request.Query["ReturnUrl"].FirstOrDefault();

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
            
                        return Redirect(returnUrl);
                    }
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Falha no login!");
            return View(model);
        }

        // Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

        
            var userExists = await _userHelper.GetUserByEmailAsync(model.Username!);
            if (userExists != null)
            {
                ModelState.AddModelError("Username", "Email já está registado.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Username,
                UserName = model.Username,
                PhoneNumber = model.PhoneNumber
            };

      
            var result = await _userHelper.AddUserAsync(user, model.Password!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _userHelper.AddUserToRoleAsync(user, "User");

            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = System.Net.WebUtility.UrlEncode(token);

            var tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userId = user.Id,
                token = encodedToken
            }, protocol: HttpContext.Request.Scheme);


            var response = await _mailHelper.SendEmailAsync(model.Username!, "Confirmação de Email",
                $"<h1>Confirmação de Email</h1> Para ativar sua conta, clique aqui: <a href='{tokenLink}'>Confirmar Email</a>");

            if (response)
            {
                ViewBag.Message = "Utilizador criado com sucesso! Verifique o seu email para confirmar a conta.";
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Não foi possível enviar o email de confirmação.");
            }

            return View(model);
        }


        // GET: ConfirmEmail
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                ViewBag.Message = "Utilizador não encontrado.";
                return View();
            }

            var decodedToken = System.Net.WebUtility.UrlDecode(token);
            var result = await _userHelper.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                await _userHelper.LogoutAsync();
                TempData["SuccessMessage"] = "Email confirmado com sucesso. Pode agora iniciar sessão.";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Message = "Erro ao confirmar o email. O token pode ser inválido ou expirado.";
                return View();
            }
        }


        // GET: RecoverPassword
        [AllowAnonymous]
        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

  
            var user = await _userHelper.GetUserByEmailAsync(model.Email!);
            if (user != null)
            {
                var token = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var encodedToken = System.Net.WebUtility.UrlEncode(token);

                var link = Url.Action("ResetPassword", "Account", new
                {
                    userId = user.Id,
                    token = encodedToken,
                    email = user.Email
                }, protocol: HttpContext.Request.Scheme);

       
                var emailResponse = await _mailHelper.SendEmailAsync(model.Email!, "Password Reset",
                    $"Para redefinir a sua password, clique aqui: <a href='{link}'>Redefinir Password</a>");

                if (!emailResponse)
                {
                    ModelState.AddModelError(string.Empty, "Falha ao enviar o e-mail. Por favor, tente novamente mais tarde.");
                    return View(model);
                }
            }

            ViewBag.Message = "Instruções para recuperar sua senha foram enviadas, caso o e-mail exista em nosso sistema.";
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string token, string email)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token,
                Email = email
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userHelper.GetUserByEmailAsync(model.Email!);
            if (user == null || user.Id != model.UserId)
            {
                return RedirectToAction("Login");
            }

            var decodedToken = System.Net.WebUtility.UrlDecode(model.Token);
            var result = await _userHelper.ResetPasswordAsync(user, decodedToken!, model.NewPassword!);

            if (result.Succeeded)
            {
                // Aqui pode definir uma flag, se o seu modelo User tiver algo como 'PasswordInicialDefinida'
                // user.PasswordInicialDefinida = true;
                // await _userHelper.UpdateUserAsync(user);

                ViewBag.Message = "Password redefinida com sucesso. Pode agora iniciar sessão.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ImageId = user.ImageId
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            if (model.ImageFile != null)
            {
                var imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                user.ImageId = imageId;
            }

            var result = await _userHelper.UpdateUserAsync(user);

            if (result.Succeeded)
            {
                ViewBag.Message = "Perfil atualizado com sucesso.";
                model.ImageId = user.ImageId;
                return View(model);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }


        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Utilizador não encontrado.");
                return View(model);
            }

            var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword!, model.NewPassword!);

            if (result.Succeeded)
            {
                ViewBag.Message = "Password alterada com sucesso.";
                return View(model); // Retorna a vista com a mensagem de sucesso
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault()?.Description!);
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult NotAuthorized()
        {
            return View();
        }



    }
}
