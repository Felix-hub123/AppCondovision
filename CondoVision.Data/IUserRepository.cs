using CondoVision.Data.Entities;
using CondoVision.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface IUserRepository 
    {

        Task<IEnumerable<User>> GetAllAsync();
        /// Retorna um IQueryable para consultar todos os usuários não deletados.
        /// </summary>
        /// <returns>IQueryable<User> para consultas adicionais.</returns>
        IQueryable<User> GetAllQueryable();

        /// <summary>
        /// Busca um usuário pelo ID, lançando exceção se não encontrado.
        /// </summary>
        /// <param name="id">O ID do usuário.</param>
        /// <returns>O usuário encontrado.</returns>
        /// <exception cref="KeyNotFoundException">Lançado quando o usuário não é encontrado.</exception>
        Task<User> GetByIdAsync(string id);

        /// <summary>
        /// Adiciona um novo usuário ao repositório.
        /// </summary>
        /// <param name="entity">O usuário a ser adicionado.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        Task AddAsync(User entity);

        /// <summary>
        /// Atualiza um usuário existente no repositório.
        /// </summary>
        /// <param name="entity">O usuário a ser atualizado.</param>
        void Update(User entity);

        /// <summary>
        /// Remove um usuário marcando-o como deletado (soft delete).
        /// </summary>
        /// <param name="entity">O usuário a ser removido.</param>
        void Remove(User entity);

        /// <summary>
        /// Salva todas as mudanças pendentes no contexto do banco de dados.
        /// </summary>
        /// <returns>O número de entidades afetadas.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Cria um novo usuário e salva as mudanças.
        /// </summary>
        /// <param name="entity">O usuário a ser criado.</param>
        /// <returns>O usuário criado.</returns>
        Task<User> CreateAsync(User entity);

        /// <summary>
        /// Atualiza um usuário existente e salva as mudanças.
        /// </summary>
        /// <param name="entity">O usuário a ser atualizado.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        Task UpdateAsync(User entity);

        /// <summary>
        /// Deleta um usuário marcando-o como deletado e salva as mudanças.
        /// </summary>
        /// <param name="entity">O usuário a ser deletado.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        Task DeleteAsync(User entity);

        /// <summary>
        /// Verifica se um usuário existe pelo ID.
        /// </summary>
        /// <param name="id">O ID do usuário.</param>
        /// <returns>True se o usuário existe, False caso contrário.</returns>
        Task<bool> ExistsAsync(string id);

        /// <summary>
        /// Obtém o nome de usuário (UserName) por ID.
        /// </summary>
        /// <param name="userId">O ID do usuário.</param>
        /// <returns>O nome de usuário ou null se não encontrado.</returns>
        Task<string?> GetUserNameByIdAsync(string userId);
    }
}
