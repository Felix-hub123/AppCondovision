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
    public interface IGenericRepository<T> where T : class, IEntity
    {


        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Obtém um IQueryable de todas as entidades no conjunto, com filtro de soft delete.
        /// </summary>
        /// <returns>Um IQueryable que representa as entidades no conjunto.</returns>
        IQueryable<T> GetAllQueryable();

        /// <summary>
        /// Obtém uma entidade pelo seu identificador de forma assíncrona.
        /// </summary>
        /// <param name="id">ID da entidade.</param>
        /// <returns>A entidade correspondente ou null.</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Obtém uma entidade pelo seu identificador de forma assíncrona, ignorando o filtro de soft delete se especificado.
        /// </summary>
        /// <param name="id">ID da entidade.</param>
        /// <param name="ignoreSoftDelete">Indica se o filtro de soft delete deve ser ignorado.</param>
        /// <returns>A entidade correspondente ou null.</returns>
        Task<T> GetByIdAsync(int id, bool ignoreSoftDelete = false);

        /// <summary>
        /// Adiciona uma nova entidade de forma assíncrona.
        /// </summary>
        /// <param name="entity">A entidade a ser adicionada.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Atualiza uma entidade existente.
        /// </summary>
        /// <param name="entity">A entidade a ser atualizada.</param>
        void Update(T entity);

        /// <summary>
        /// Remove uma entidade (marca como excluída para soft delete).
        /// </summary>
        /// <param name="entity">A entidade a ser removida.</param>
        void Remove(T entity);

        /// <summary>
        /// Salva todas as alterações feitas neste contexto na base de dados de forma assíncrona.
        /// </summary>
        /// <returns>O número de entradas de estado escritas na base de dados.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Cria uma nova entidade de forma assíncrona.
        /// </summary>
        /// <param name="entity">A entidade a ser criada.</param>
        /// <returns>A entidade criada.</returns>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Atualiza uma entidade existente de forma assíncrona.
        /// </summary>
        /// <param name="entity">A entidade a ser atualizada.</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Remove uma entidade de forma assíncrona (marca como excluída para soft delete).
        /// </summary>
        /// <param name="entity">A entidade a ser removida.</param>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Verifica se uma entidade com o ID especificado existe de forma assíncrona.
        /// </summary>
        /// <param name="id">ID da entidade.</param>
        /// <returns>Retorna true se a entidade existe, false caso contrário.</returns>
        Task<bool> ExistsAsync(int id);


        Task<List<T>> GetRecentAsync(int take = 5);

    }


}
   
    

