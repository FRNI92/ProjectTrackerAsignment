using Data.Interfaces;
using System.Linq.Expressions;

namespace Business.Services;

public abstract class BaseService<TEntity> where TEntity : class
{
    private readonly IBaseRepository<TEntity> _repository;

    protected BaseService(IBaseRepository<TEntity> repository)
    {
        _repository = repository;
    }
    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        return await _repository.CreateAsync(entity);
    }

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await _repository.GetAsync(expression);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<TEntity> UpdateAsync(Expression<Func<TEntity, bool>> expression, TEntity updatedEntity)
    {
        return await _repository.UpdateAsync(expression, updatedEntity);
    }


    public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await _repository.DeleteAsync(expression);
    }
}
