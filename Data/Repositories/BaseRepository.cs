using Data.Contexts;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories;

public abstract class BaseRepository<TEntity>(DataContext context) : IBaseRepository<TEntity> where TEntity : class
{
    private readonly DataContext _context = context;
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();



    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        Console.WriteLine("Creating Something");
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        else
        {
            //await _dbSet.AddAsync(entity);
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        Console.WriteLine("BaseRepository: Fetching all entities In...");
        return await _dbSet.ToListAsync();
    }


    // x => x.Id == Id , x => x.Email == Email
    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
    {
        Console.WriteLine("BaseRepository: Fetching single entity...");
        var entity = await _dbSet.FirstOrDefaultAsync(expression);

        if (entity == null)
            throw new KeyNotFoundException("Entity not found");

        return entity;
    }

    public async Task<TEntity> UpdateAsync(Expression<Func<TEntity, bool>> expression, TEntity updatedEntity)
    {

        var existingEntity = await _dbSet.FirstOrDefaultAsync(expression);
        if (existingEntity != null && updatedEntity != null)
        {
            _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
            await _context.SaveChangesAsync();
            return existingEntity;
        }
        return null!;
       
     

        //_dbSet.Entry(entity).State = EntityState.Modified;//ändrade till detta från update så att det inte skapas en ny entitet
       // await _context.SaveChangesAsync();
        //return entity;
    }
    public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression)
    {
        var entity = _dbSet.FirstOrDefault(expression);
        if (entity != null)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;

       // Console.WriteLine("Deleting Customer");
        //_dbSet.Remove(entity);//tar bort entiteten
        //await _context.SaveChangesAsync();//sparar databasen
    }
}
