using Data.Contexts;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Data.Repositories;

public abstract class BaseRepository<TEntity>(DataContext context) : IBaseRepository<TEntity> where TEntity : class
{
    private readonly DataContext _context = context;
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    private IDbContextTransaction _transaction = null!;

    public virtual async Task BeginTransactionAsync()
    {
        if(_transaction == null)
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }
    }

    public virtual async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }

    }

    public virtual async Task RollBackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }
    }



    public virtual async Task<bool> AddAsync(TEntity entity)
    {
        try
        {
            Console.WriteLine("Adding Something");
            if (entity == null)
            {
                return false;
            }
            else
            {
                await _dbSet.AddAsync(entity);
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.Write($"Error In AddAsync:{ex.Message}");
            return false;
        }
    }


    public virtual async Task<int> SaveAsync()
    {
        try
        {
            Console.WriteLine("Saving Something");
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Debug.Write($"Error In SaveAsync: {ex.Message}");
            return 0; // Returnera 0 vid fel istället för false, typ som en bool
        }
    }


    public virtual async Task<bool> CreateAsync(TEntity entity)
    {
        try
        {
            Console.WriteLine("Creating Something");
            if (entity == null)
            {
                return false;
            }
            else
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.Write($"Error In CreateAsync:{ex.Message}");
            return false;
        }
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            var allEntities = await _dbSet.ToListAsync();
            if (allEntities == null)
                return null!;
            Debug.WriteLine("BaseRepository: Fetching all entities In...");
            return allEntities;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error In GetAllAsync:{ex.Message} {ex.StackTrace}");
            return null!;
        }
    }

    public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
    {
        try
        {
            Console.WriteLine("BaseRepository: Fetching single entity...");
            var entity = await _dbSet.FirstOrDefaultAsync(expression);

            if (entity == null)
                return null!; 

            return entity;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error In GetAsync:{ex.Message} {ex.StackTrace}");
            return null!;
        }
    }

    //transactionManagement
    public virtual async Task<TEntity> TransactionUpdateAsync(Expression<Func<TEntity, bool>> expression, TEntity updatedEntity)
    {

        try
        {
            var existingEntity = await _dbSet.FirstOrDefaultAsync(expression);
            if (existingEntity != null && updatedEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);               
                return existingEntity;
            }
            return null!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateAsync:{ex.Message}");
            return null!;
        }

    }
    //transactionManagement


    public virtual async Task<TEntity> UpdateAsync(Expression<Func<TEntity, bool>> expression, TEntity updatedEntity)
    {

        try
        {
            var existingEntity = await _dbSet.FirstOrDefaultAsync(expression);
            if (existingEntity != null && updatedEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
                //await _context.SaveChangesAsync();//tog bort savechangesasyn härifrån för att den är utbruten
                return existingEntity;
            }
            return null!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateAsync:{ex.Message}");
            return null!;
        }

    }

    //transactionManagement
    public virtual async Task<bool> RemoveAsync(Expression<Func<TEntity, bool>> expression)
    {
        try
        {
            var entity = await _dbSet.FirstOrDefaultAsync(expression);
            if (entity != null)
            {
                _context.Remove(entity);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error With DeleteAsync{ex.Message}");
            return false;
        }
    }
    //transactionManagement

    public virtual async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression)
    {
        try
        {
            var entity = await _dbSet.FirstOrDefaultAsync(expression);
            if (entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();//den här är utbruten, ändra anropsvägen
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error With DeleteAsync{ex.Message}");
            return false;
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
            Debug.WriteLine($"Error with DoesEntityExistAsync{ex.Message}");
            return false;
        }
    }
}
