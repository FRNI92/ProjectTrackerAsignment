using Data.Contexts;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories;

public abstract class BaseRepository<TEntity>(DataContext context) : IBaseRepository<TEntity> where TEntity : class
{
    private readonly DataContext _context = context;
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        try
        {
            Console.WriteLine("Creating Something");
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), $"{typeof(TEntity).Name} cannot be null.");
            }
            else
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error In CreateAsync:{ex.Message}");
            throw;
        }
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            Console.WriteLine("BaseRepository: Fetching all entities In...");
            return await _dbSet.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error In GetAllAsync:{ex.Message}");
            throw;
        }
    }

    public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
    {
        try
        {
            Console.WriteLine("BaseRepository: Fetching single entity...");
            var entity = await _dbSet.FirstOrDefaultAsync(expression);

            if (entity == null)
                throw new KeyNotFoundException("Entity not found");

            return entity;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error In GetAsync:{ex.Message}");
            throw;
        }
    }

    public virtual async Task<TEntity> UpdateAsync(Expression<Func<TEntity, bool>> expression, TEntity updatedEntity)
    {

        try
        {
            var existingEntity = await _dbSet.FirstOrDefaultAsync(expression);
            if (existingEntity != null && updatedEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
                await _context.SaveChangesAsync();
                return existingEntity;
            }
            return null!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateAsync:{ex.Message}");
            throw;
        }

    }
    public virtual async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression)
    {
        try
        {
            var entity = await _dbSet.FirstOrDefaultAsync(expression);
            if (entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error With DeleteAsync{ex.Message}");
            throw;
        }
    }

    public virtual async Task<bool> DoesEntityExistAsync(Expression<Func<TEntity, bool>> expression)
    {
        try
        {
            return await _dbSet.AnyAsync(expression);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error with DoesEntityExistAsync{ex.Message}");
            throw;
        }
    }
}
