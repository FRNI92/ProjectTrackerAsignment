using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Data_Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Tests.Repositories;

//public class BaseRepository_Tests : Test_Base
//{
//    private readonly IBaseRepository<ProjectEntity> _baseRepository;

//    public BaseRepository_Tests() : base()
//    {
//        _baseRepository = new BaseRepository<ProjectEntity>(_context); // Specificera entitetstypen
//    }





//    public async Task AddAsync_ShouldAddToDatabase()
//    {
//        // ARRANGE
//        var project = new ProjectEntity
//        {
//            Id = 1,
//            ProjectNumber = "P-101",
//            Name = "Test Project"
//        };

//        // ACT
//        var result = await _baseRepository.AddAsync(project);
//        await _baseRepository.SaveAsync(); // Viktigt för att spara i InMemoryDatabase

//        // ASSERT
//        Assert.True(result);
//        var savedProject = await _context.Projects.FindAsync(1);
//        Assert.NotNull(savedProject);
//        Assert.Equal("Test Project", savedProject.Name);
//    }



        //Task<bool> AddAsync(TEntity entity);
        //Task<int> SaveAsync();
        //Task<IEnumerable<TEntity>> GetAllAsync();
        //Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression);
        //Task<bool> RemoveAsync(Expression<Func<TEntity, bool>> expression);
        //Task<TEntity> TransactionUpdateAsync(Expression<Func<TEntity, bool>> expression, TEntity updatedEntity);
        //Task<bool> DoesEntityExistAsync(Expression<Func<TEntity, bool>> expression);

