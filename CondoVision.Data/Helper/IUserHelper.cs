using CondoVision.Data.Entities;
using CondoVision.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CondoVision.Data.Helper
{
    /// <summary>
    /// Interface que define métodos auxiliares para operações relacionadas com utilizadores,
    /// incluindo autenticação, autorização, criação, atualização e gestão de roles e tokens.
    /// </summary>
    public interface IUserHelper
    {
        /// <summary>
        /// Cria um novo utilizador com a password definida.
        /// </summary>
        Task<IdentityResult> AddUserAsync(User user, string password);

        /// <summary>
        /// Obtém um utilizador pelo email.
        /// </summary>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Adiciona um utilizador a uma role.
        /// </summary>
        Task<IdentityResult> AddUserToRoleAsync(User user, string role);

        /// <summary>
        /// Verifica se um utilizador pertence a uma role.
        /// </summary>
        Task<bool> IsUserInRoleAsync(User user, string roleName);

        /// <summary>
        /// Verifica se uma role existe, e cria se não existir.
        /// </summary>
        Task CheckRoleAsync(string roleName);

        /// <summary>
        /// Obtém um utilizador pelo seu identificador.
        /// </summary>
        Task<User?> GetUserByIdAsync(string id);

        /// <summary>
        /// Obtém o utilizador a partir de informações do ClaimsPrincipal.
        /// </summary>
        Task<User?> GetUserAsync(ClaimsPrincipal userClaims);

        /// <summary>
        /// Atualiza os dados de um utilizador existente.
        /// </summary>
        Task<IdentityResult> UpdateUserAsync(User user);

        /// <summary>
        /// Valida a password do utilizador sem efetuar login.
        /// </summary>
        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        /// <summary>
        /// Efetua logout do utilizador atual.
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Efetua o login do utilizador.
        /// </summary>
        Task<SignInResult> LoginAsync(LoginViewModel model);

        /// <summary>
        /// Obtém o identificador do utilizador a partir de um ClaimsPrincipal.
        /// </summary>
        string? GetUserId(ClaimsPrincipal user);

        /// <summary>
        /// Altera a password do utilizador.
        /// </summary>
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        /// <summary>
        /// Gera um token para confirmação de email para o utilizador.
        /// </summary>
        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        /// <summary>
        /// Confirma o email do utilizador com o token fornecido.
        /// </summary>
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        /// <summary>
        /// Gera um token para redefinição de password do utilizador.
        /// </summary>
        Task<string> GeneratePasswordResetTokenAsync(User user);

        /// <summary>
        /// Redefine a password do utilizador usando o token de reset.
        /// </summary>
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        /// <summary>
        /// Obtém todos os utilizadores que pertencem a uma role especificada,
        /// excluindo os que foram logicamente eliminados.
        /// </summary>
        Task<IEnumerable<User>> GetUsersInRoleAsync(string roleName);

        /// <summary>
        /// Verifica se o email do utilizador está confirmado.
        /// </summary>
        Task<bool> IsEmailConfirmedAsync(User user);

        /// <summary>
        /// Obtém as roles atribuídas ao utilizador.
        /// </summary>
        Task<IList<string>> GetUserRolesAsync(User user);

        /// <summary>
        /// Marca um utilizador como logicamente eliminado e atualiza na base de dados.
        /// </summary>
        Task<IdentityResult> DeleteUserAsync(User user);

        /// <summary>
        /// Obtém um utilizador pelo seu ID, incluindo detalhes da Companhia, Unidades Detidas e Condomínios Geridos.
        /// </summary>
        Task<User?> GetUserWithDetailsAsync(string userId);

        /// <summary>
        /// Obtém o identificador do utilizador a partir do HttpContext atual.
        /// </summary>
        string? GetUserId();

    }
}

