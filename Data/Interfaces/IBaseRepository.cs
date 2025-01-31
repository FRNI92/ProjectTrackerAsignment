﻿using System.Linq.Expressions;

namespace Data.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<TEntity> CreateAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression);

        Task<TEntity> UpdateAsync(Expression<Func<TEntity, bool>> expression, TEntity updatedEntity);
        //task:gör detta. <TEntity>: få tillbaka den här typen. MetodNamn. "tål" bara en enda object av typen TEntity
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression);
        //task: gör detta. Metodnamn. Tål bara en av typen TEntity
    }
}