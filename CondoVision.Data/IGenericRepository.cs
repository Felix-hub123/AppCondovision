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
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Adiciona uma nova entidade à base de dados.
        /// </summary>
        /// <param name="entity">A entidade a ser adicionada.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Atualiza uma entidade existente na base de dados.
        /// </summary>
        /// <param name="entity">A entidade a ser atualizada.</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Elimina uma entidade da base de dados (soft delete).
        /// </summary>
        /// <param name="entity">A entidade a ser eliminada.</param>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Elimina uma entidade da base de dados (soft delete) pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da entidade a ser eliminada.</param>
        Task DeleteAsync(int id);

        /// <summary>
        /// Obtém uma entidade pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da entidade.</param>
        /// <returns>A entidade encontrada ou null se não existir.</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Obtém todas as entidades do tipo.
        /// </summary>
        /// <returns>Uma coleção de todas as entidades.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Salva todas as alterações pendentes no DataContext.
        /// </summary>
        /// <returns>O número de estados de objeto escritos na base de dados.</returns>
        Task<int> CompleteAsync();
    }


}
   
    

