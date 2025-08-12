using CondoVision.Models.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CondoVision.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        protected readonly DataContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DataContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }


        /// <summary>
        /// Obtém todas as entidades do tipo que não foram logicamente eliminadas.
        /// </summary>
        /// <returns>Uma coleção de todas as entidades não eliminadas.</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.Where(e => !e.WasDeleted).ToListAsync();
        }

        /// <summary>
        /// Obtém uma entidade pelo seu ID, garantindo que não foi logicamente eliminada.
        /// </summary>
        /// <param name="id">O ID da entidade.</param>
        /// <returns>A entidade encontrada ou null se não existir ou estiver eliminada.</returns>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.WasDeleted);
        }

        /// <summary>
        /// Adiciona uma nova entidade à base de dados.
        /// </summary>
        /// <param name="entity">A entidade a ser adicionada.</param>
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync(); // Salva as alterações imediatamente após a adição
        }

        /// <summary>
        /// Atualiza uma entidade existente na base de dados.
        /// Este é o ÚNICO método de atualização.
        /// </summary>
        /// <param name="entity">A entidade a ser atualizada.</param>
        public async Task UpdateAsync(T entity) // <--- MÉTODO ÚNICO E CORRETO
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(); 
        }

        /// <summary>
        /// Marca uma entidade como logicamente eliminada e salva as alterações.
        /// </summary>
        /// <param name="entity">A entidade a ser eliminada.</param>
        public async Task DeleteAsync(T entity) // <--- MÉTODO DeleteAsync (recebe entidade)
        {
            entity.WasDeleted = true;
            await UpdateAsync(entity); // Chama o método UpdateAsync para persistir a alteração
        }

        /// <summary>
        /// Marca uma entidade como logicamente eliminada pelo ID e salva as alterações.
        /// </summary>
        /// <param name="id">O ID da entidade a ser eliminada.</param>
        public async Task DeleteAsync(int id) // <--- MÉTODO DeleteAsync (recebe ID)
        {
            var entity = await GetByIdAsync(id); // Obtém a entidade
            if (entity != null)
            {
                entity.WasDeleted = true; // Marca para soft delete
                await UpdateAsync(entity); // Chama o método UpdateAsync para persistir a alteração
            }
        }

        /// <summary>
        /// Salva todas as alterações pendentes no DataContext.
        /// (Pode ser removido se cada operação de CRUD salvar imediatamente)
        /// </summary>
        /// <returns>O número de estados de objeto escritos na base de dados.</returns>
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retorna um IQueryable para consultas mais complexas, excluindo entidades eliminadas.
        /// </summary>
        /// <returns>Um IQueryable das entidades não eliminadas.</returns>
        public IQueryable<T> GetQueryable()
        {
            return _dbSet.Where(e => !e.WasDeleted);
        }

        /// <summary>
        /// Obtém todas as entidades com propriedades de navegação incluídas, excluindo entidades eliminadas.
        /// </summary>
        /// <param name="includeExpressions">Expressões para incluir propriedades de navegação.</param>
        /// <returns>Uma coleção de entidades com as propriedades incluídas.</returns>
        public async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> query = _dbSet;
            foreach (var includeExpression in includeExpressions)
            {
                query = query.Include(includeExpression);
            }
            return await query.Where(e => !e.WasDeleted).ToListAsync();
        }

        /// <summary>
        /// Obtém uma entidade pelo ID com propriedades de navegação incluídas, garantindo que não foi logicamente eliminada.
        /// </summary>
        /// <param name="id">O ID da entidade.</param>
        /// <param name="includeExpressions">Expressões para incluir propriedades de navegação.</param>
        /// <returns>A entidade encontrada com as propriedades incluídas ou null.</returns>
        public async Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> query = _dbSet;
            foreach (var includeExpression in includeExpressions)
            {
                query = query.Include(includeExpression);
            }
            return await query.Where(e => e.Id == id && !e.WasDeleted).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Executa uma consulta IQueryable e retorna os resultados.
        /// </summary>
        /// <param name="query">A consulta IQueryable a ser executada.</param>
        /// <returns>Uma coleção de entidades.</returns>
        public async Task<IEnumerable<T>> GetAllAsync(IQueryable<T> query)
        {
            return await query.ToListAsync();
        }
    }


}
