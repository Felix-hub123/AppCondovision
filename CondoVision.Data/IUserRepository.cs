using CondoVision.Data.Entities;
using CondoVision.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface IUserRepository 
    {
        /// <summary>
        /// Obtém um IQueryable de todos os utilizadores, com filtro de soft delete.
        /// </summary>
        /// <returns>Um IQueryable que representa os utilizadores no conjunto.</returns>
        IQueryable<User> GetAllQueryable();

        /// <summary>
        /// Obtém um utilizador pelo seu identificador de forma assíncrona.
        /// </summary>
        /// <param name="id">ID do utilizador (string, conforme IdentityUser).</param>
        /// <returns>O utilizador correspondente ou null.</returns>
        Task<User> GetByIdAsync(string id);

        /// <summary>
        /// Adiciona um novo utilizador de forma assíncrona.
        /// </summary>
        /// <param name="entity">O utilizador a ser adicionado.</param>
        Task AddAsync(User entity);

        /// <summary>
        /// Atualiza um utilizador existente.
        /// </summary>
        /// <param name="entity">O utilizador a ser atualizado.</param>
        void Update(User entity);

        /// <summary>
        /// Remove um utilizador (marca como excluído para soft delete).
        /// </summary>
        /// <param name="entity">O utilizador a ser removido.</param>
        void Remove(User entity);

        /// <summary>
        /// Salva todas as alterações feitas neste contexto na base de dados de forma assíncrona.
        /// </summary>
        /// <returns>O número de entradas de estado escritas na base de dados.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Cria um novo utilizador de forma assíncrona.
        /// </summary>
        /// <param name="entity">O utilizador a ser criado.</param>
        /// <returns>O utilizador criado.</returns>
        Task<User> CreateAsync(User entity);

        /// <summary>
        /// Atualiza um utilizador existente de forma assíncrona.
        /// </summary>
        /// <param name="entity">O utilizador a ser atualizado.</param>
        Task UpdateAsync(User entity);

        /// <summary>
        /// Remove um utilizador de forma assíncrona (marca como excluído para soft delete).
        /// </summary>
        /// <param name="entity">O utilizador a ser removido.</param>
        Task DeleteAsync(User entity);

        /// <summary>
        /// Verifica se um utilizador com o ID especificado existe de forma assíncrona.
        /// </summary>
        /// <param name="id">ID do utilizador (string, conforme IdentityUser).</param>
        /// <returns>Retorna true se o utilizador existe, false caso contrário.</returns>
        Task<bool> ExistsAsync(string id);

        /// <summary>
        /// Obtém o nome de utilizador (UserName) de um utilizador com base no ID fornecido de forma assíncrona.
        /// </summary>
        /// <param name="userId">ID do utilizador (string, conforme IdentityUser).</param>
        /// <returns>O nome de utilizador ou null se não encontrado.</returns>
        Task<string?> GetUserNameByIdAsync(string userId);
    }
}
