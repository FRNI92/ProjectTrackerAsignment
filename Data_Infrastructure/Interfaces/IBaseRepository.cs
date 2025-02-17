using System.Linq.Expressions;

namespace Data_Infrastructure.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollBackTransactionAsync();

        Task<bool> AddAsync(TEntity entity);
        Task<int> SaveAsync();

        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression);

        Task<bool> RemoveAsync(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> TransactionUpdateAsync(Expression<Func<TEntity, bool>> expression, TEntity updatedEntity);
      
        Task<bool> DoesEntityExistAsync(Expression<Func<TEntity, bool>> expression);
    }
}