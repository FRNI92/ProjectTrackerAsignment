using Data_Infrastructure.Contexts;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Data_Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Xunit.Sdk;

namespace Tests.Repositories;

public class ProjectRepository_Tests : Test_Base
{
    private readonly IProjectRepository _projectRepository;

    public ProjectRepository_Tests() : base() 
    {
        _projectRepository = new ProjectRepository(_context);
    }

    [Fact]
    public async Task Add_Async_ShouldReturnTrue_WhenSavedToDatabase()
    {
        //arrange
        var entity = new ProjectEntity
        {
            Id = 1,
            ProjectNumber = "P-101",
            Name = "Projekt A",
            Description = "Första projektet",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 9, 1),
            StatusId = 1,
            CustomerId = 1,
            ServiceId = 1,
            EmployeeId = 1,
            Duration = 10,
            //TotalPrice = 5000 is calculated by service
        };

        //act
        var result = await _projectRepository.AddAsync(entity);

        //assert

        Assert.True(result);
        var savedEntity = await _context.Projects.FindAsync(entity.Id);
        Assert.NotNull(savedEntity);
        Assert.Equal(entity.ProjectNumber, savedEntity.ProjectNumber);
        Assert.Equal(entity.Name, savedEntity.Name);
        Assert.Equal(entity.Description, savedEntity.Description);

    }

    [Fact]
    public async Task GetAllAsync_ShouldIncludeFromOtherEnitites()
    {
        //arrange
        var status = new StatusEntity { Id = 1, Name = "Active" };
        var employee = new EmployeesEntity { 
            Id = 1, 
            RoleId = 1,
            FirstName = "John Doe", 
            LastName = "Nilsson", 
            Email = "Fredrik@domain.com" };
        var customer = new CustomersEntity { 
            Id = 1, 
            Name = "ACME Corp", 
            Email = "Customer@domain.com", 
            PhoneNumber = "123455677" };
        var service = new ServiceEntity { Id = 1, Name = "Consulting", Price = 1000 };

        _context.Statuses.Add(status);
        _context.Employees.Add(employee);
        _context.Customers.Add(customer);
        _context.Services.Add(service);


        var project = new ProjectEntity
        {
            Id = 1,
            ProjectNumber = "P-101",
            Name = "Test Project",
            StatusId = 1,
            EmployeeId = 1,
            CustomerId = 1,
            ServiceId = 1
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var repository = new ProjectRepository(_context);

        // act 
        var result = await repository.GetAllAsync();

        // assert
        Assert.NotNull(result);
        Assert.Single(result);

        var fetchedProject = result.First();
        Assert.Equal("Test Project", fetchedProject.Name);
        Assert.NotNull(fetchedProject.Status);
        Assert.Equal("Active", fetchedProject.Status.Name);
        Assert.NotNull(fetchedProject.Employee);
        Assert.Equal("John Doe", fetchedProject.Employee.FirstName);
        Assert.Equal("Nilsson", fetchedProject.Employee.LastName);
        Assert.NotNull(fetchedProject.Customer);
        Assert.Equal("ACME Corp", fetchedProject.Customer.Name);
        Assert.NotNull(fetchedProject.Service);
        Assert.Equal("Consulting", fetchedProject.Service.Name);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnCorrectEntity()
    {
        //arrange
        var project1 = new ProjectEntity { Id = 1, ProjectNumber = "P-101", Name = "Project One" };
        var project2 = new ProjectEntity { Id = 2, ProjectNumber = "P-102", Name = "Project Two" };

        _context.Projects.Add(project1);
        _context.Projects.Add(project2);
        await _context.SaveChangesAsync(); 

        //act
        var result = await _projectRepository.GetAsync(p => p.Id == 2); 

        //assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal("P-102", result.ProjectNumber);
        Assert.Equal("Project Two", result.Name);
    }


    [Fact]
    public async Task SaveAsync_ShouldSaveAndReturnInt()
    {
        //arrange
        var project1 = new ProjectEntity { 
            Id = 1, 
            ProjectNumber = "TestNumber", 
            Name = "Test1" };

        await _projectRepository.AddAsync(project1);
        var saveResult = await _projectRepository.SaveAsync();

        //act
        var savedProject = await _projectRepository.GetAsync(p => p.Id == 1);

        //Assert
        Assert.NotNull(savedProject);
        Assert.Equal(1, saveResult);
        Assert.Equal(project1.Id, savedProject.Id);
        Assert.Equal("TestNumber", savedProject.ProjectNumber);
        Assert.Equal("Test1", savedProject.Name);
    }

    [Fact] 
    public async Task RemoveAsync_ShouldRemoveAndReturnBool()
    {
        //arrange
        var testProject = new ProjectEntity
        {
            Id = 1,
            ProjectNumber = "test1",
            Name = "testproject"
        };
        await _projectRepository.AddAsync(testProject);
        await _projectRepository.SaveAsync();

        //act
        var loadedEntity = await _projectRepository.GetAsync(p => p.Id == testProject.Id);
        var result = await _projectRepository.RemoveAsync(p => p.Id == testProject.Id);
        await _projectRepository.SaveAsync();

        // assert
        var deletedEntity = await _projectRepository.GetAsync(p => p.Id == testProject.Id);
        Assert.True(result);
        Assert.Null(deletedEntity);
    }


    [Fact]
    public async Task TransactionUpdateAsync_ShouldUpdateEntity_AndReturnEntity()
    {
        //arrange
        var notUpdatedProject = new ProjectEntity
        {
            Id = 1,
            ProjectNumber = "Test1",
            Name = "TestProject"
        };
        await _projectRepository.AddAsync(notUpdatedProject);
        await _projectRepository.SaveAsync();

        //act 
        var newDataProject = new ProjectEntity
        {
            Id = notUpdatedProject.Id,
            ProjectNumber = notUpdatedProject.ProjectNumber,
            Name = "UpdatedTestProject"
        };

        var result = await _projectRepository.TransactionUpdateAsync(p => p.Id == notUpdatedProject.Id, newDataProject);

        //assert
        Assert.NotNull(result);
        Assert.Equal("UpdatedTestProject", result.Name);
        Assert.Equal(notUpdatedProject.Name, result.Name);
    }

    [Fact]
    public async Task DoesEntityExistAsync_ShouldReturnBool()
    {
        //arrange
        var existingTest = new ProjectEntity
        {
            Id = 1,
            ProjectNumber = "testnumber",
            Name = "testname"
        };
        await _projectRepository.AddAsync(existingTest);
        await _projectRepository.SaveAsync();

        var newTest = new ProjectEntity
        {
            Id = 2,
            ProjectNumber = "testnumber",
            Name = "testname"
        };

        //act
        var isExisting = await _projectRepository.DoesEntityExistAsync(p => p.ProjectNumber == newTest.ProjectNumber);

        //assert
        Assert.True(isExisting);

    }





    //Task<bool> DoesEntityExistAsync(Expression<Func<TEntity, bool>> expression); test is done
    //Task<TEntity> TransactionUpdateAsync(Expression<Func<TEntity, bool>> expression, TEntity updatedEntity);
    //Task<bool> RemoveAsync(Expression<Func<TEntity, bool>> expression); test is done
    //Task<int> SaveAsync(); test is done
    //Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression); test is done
    //Task<bool> AddAsync(TEntity entity); test is done
    //Task<IEnumerable<TEntity>> GetAllAsync(); test is done
}
