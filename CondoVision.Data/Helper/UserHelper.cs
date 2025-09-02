using CondoVision.Data.Entities;
using CondoVision.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CondoVision.Data.Helper
{
    /// <summary>
    /// Helper para operações relacionadas a utilizadores, autenticação e gestão de roles,
    /// utilizando a infraestrutura do Identity do ASP.NET Core.
    /// </summary>
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;


        /// <summary>
        /// Inicializa uma nova instância do <see cref="UserHelper"/> com as dependências necessárias.
        /// </summary>
        /// <param name="userManager">UserManager para gerir utilizadores.</param>
        /// <param name="signInManager">SignInManager para gerir sign-in/sign-out.</param>
        /// <param name="roleManager">RoleManager para gerir roles de utilizadores.</param>
        public UserHelper(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }


        /// <summary>
        /// Cria um novo utilizador com a password definida.
        /// </summary>
        /// <param name="user">Entidade do utilizador a criar.</param>
        /// <param name="password">Password do utilizador.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }


        /// <summary>
        /// Obtém um utilizador pelo email.
        /// </summary>
        /// <param name="email">Email a procurar.</param>
        /// <returns>Usuário encontrado ou null.</returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }


        /// <summary>
        /// Adiciona um utilizador a uma role.
        /// </summary>
        /// <param name="user">Utilizador a adicionar.</param>
        /// <param name="role">Nome da role.</param>
        public async Task<IdentityResult> AddUserToRoleAsync(User user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }



        /// <summary>
        /// Verifica se um utilizador pertence a uma role.
        /// </summary>
        /// <param name="user">Utilizador a verificar.</param>
        /// <param name="roleName">Nome da role.</param>
        /// <returns>True se o utilizador está na role, caso contrário false.</returns>
        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        /// <summary>
        /// Verifica se uma role existe, e cria se não existir.
        /// </summary>
        /// <param name="roleName">Nome da role a verificar/criar.</param>
        public async Task CheckRoleAsync(string roleName)
        {
            var result = await _roleManager.RoleExistsAsync(roleName);
            if (!result)
                await _roleManager.CreateAsync(
                    new IdentityRole { Name = roleName });
        }


        /// <summary>
        /// Obtém um utilizador pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador do utilizador.</param>
        /// <returns>Utilizador encontrado ou null.</returns>
        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        /// <summary>
        /// Obtém o utilizador a partir de informações do ClaimsPrincipal.
        /// Suporta Claims padrão e token JWT (sub).
        /// </summary>
        /// <param name="userClaims">ClaimsPrincipal do utilizador.</param>
        /// <returns>Utilizador encontrado ou null.</returns>
        public async Task<User?> GetUserAsync(ClaimsPrincipal userClaims)
        {
            var userId = userClaims?.FindFirstValue(ClaimTypes.NameIdentifier)
                                   ?? userClaims?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userId))
                return null; // Alterado de null! para null

            return await GetUserByIdAsync(userId);
        }


        /// <summary>
        /// Atualiza os dados de um utilizador existente.
        /// </summary>
        /// <param name="user">Entidade do utilizador a atualizar.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Valida a password do utilizador sem efetuar login.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <param name="password">Password a validar.</param>
        /// <returns>Resultado da validação do password.</returns>
        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(
                 user, password, false);
        }


        /// <summary>
        /// Efetua logout do utilizador atual.
        /// </summary>
        /// <returns>Uma Task que representa a operação assíncrona.</returns>
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }



        /// <summary>
        /// Efetua o login do utilizador.
        /// </summary>
        /// <param name="model">ViewModel com credenciais de login.</param>
        /// <returns>Resultado do login.</returns>
        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                false
            );
        }

        /// <summary>
        /// Obtém o identificador do utilizador a partir de um ClaimsPrincipal.
        /// </summary>
        /// <param name="user">ClaimsPrincipal do utilizador.</param>
        /// <returns>Identificador do utilizador ou null.</returns>
        public string? GetUserId(ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }


        /// <summary>
        /// Altera a password do utilizador.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <param name="oldPassword">Password atual.</param>
        /// <param name="newPassword">Nova password.</param>
        /// <returns>Resultado da alteração.</returns>
        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }


        /// <summary>
        /// Gera um token para confirmação de email para o utilizador.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <returns>Token gerado.</returns>
        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }


        /// <summary>
        /// Confirma o email do utilizador com o token fornecido.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <param name="token">Token de confirmação.</param>
        /// <returns>Resultado da confirmação.</returns>
        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }


        /// <summary>
        /// Gera um token para redefinição de password do utilizador.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <returns>Token gerado para reset de password.</returns>
        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }


        /// <summary>
        /// Redefine a password do utilizador usando o token de reset.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <param name="token">Token para redefinir a password.</param>
        /// <param name="password">Nova password.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }


        /// <summary>
        /// Obtém todos os utilizadores que pertencem a uma role especificada,
        /// excluindo os que foram logicamente eliminados.
        /// </summary>
        /// <param name="roleName">Nome da role.</param>
        /// <returns>Lista de utilizadores.</returns>
        public async Task<IEnumerable<User>> GetUsersInRoleAsync(string roleName) // <--- MÉTODO CONSOLIDADO E CORRIGIDO
        {
            // Verifica se a role existe
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return new List<User>(); // Retorna lista vazia se a role não existir
            }
            // Obtém todos os utilizadores na role especificada
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            // Filtra por WasDeleted (assumindo que User implementa IEntity e tem WasDeleted)
            return users.Where(u => !u.WasDeleted).ToList();
        }

        // Removido o método GetUsersByRoleAsync duplicado

        /// <summary>
        /// Verifica se o email do utilizador está confirmado.
        /// </summary>
        /// <param name="user">Utilizador a verificar.</param>
        /// <returns>True se o email estiver confirmado, caso contrário false.</returns>
        public async Task<bool> IsEmailConfirmedAsync(User user)
        {
            return await _userManager.IsEmailConfirmedAsync(user);
        }


        /// <summary>
        /// Obtém as roles atribuídas ao utilizador.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <returns>Lista com nomes das roles.</returns>
        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }


        /// <summary>
        /// Marca um utilizador como logicamente eliminado e atualiza na base de dados.
        /// </summary>
        /// <param name="user">Utilizador a ser marcado como eliminado.</param>
        /// <returns>Resultado da operação de atualização.</returns>
        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            user.WasDeleted = true;
            return await _userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Obtém um utilizador pelo seu ID, incluindo detalhes da Companhia, Unidades Detidas e Condomínios Geridos.
        /// </summary>
        /// <param name="userId">ID do utilizador.</param>
        /// <returns>Utilizador com detalhes ou null.</returns>
        public async Task<User?> GetUserWithDetailsAsync(string userId)
        {
            return await _userManager.Users
         .Include(u => u.CondominiumUsers)
         .Include(u => u.FractionOwners)
             .ThenInclude(fo => fo.Unit)
         .FirstOrDefaultAsync(u => u.Id == userId);
        }

        // Este método GetUserId sem parâmetro user ClaimsPrincipal, usa o HttpContextAccessor
        /// <summary>
        /// Obtém o identificador do utilizador a partir do HttpContext atual.
        /// </summary>
        /// <returns>Identificador do utilizador ou null.</returns>
        public string? GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null)
                return null;

            return user.FindFirstValue(ClaimTypes.NameIdentifier); 
        }
    }
}
