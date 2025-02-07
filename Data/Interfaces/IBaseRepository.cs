using System.Linq.Expressions;

namespace Data.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<bool> CreateAsync(TEntity entity); // kan skicka flera värden <(bool, TEntity)>
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression);

        Task<TEntity> UpdateAsync(Expression<Func<TEntity, bool>> expression, TEntity updatedEntity);
        //task:gör detta. <TEntity>: få tillbaka den här typen. MetodNamn. "tål" bara en enda object av typen TEntity
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression);
        //task: gör detta. Metodnamn. Tål bara en av typen
        Task<bool> DoesEntityExistAsync(Expression<Func<TEntity, bool>> expression);
    }
}