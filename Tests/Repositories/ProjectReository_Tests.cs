using Data_Infrastructure.Contexts;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Data_Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

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
            //TotalPrice = 5000 räknas ut av servicen endå
        };

        //act
        var result = await _projectRepository.AddAsync(entity);

        //assert

        Assert.True(result);
        var savedEntity = await _context.Projects.FindAsync(entity.Id);//nbyggd metod i EFC för att hitta en entitet baserat på primärnyckel

        //gör att jag kan checka mer saker. men detta test ska egentligen bara koll true eller false
        Assert.NotNull(savedEntity);
        Assert.Equal(entity.ProjectNumber, savedEntity.ProjectNumber);
        Assert.Equal(entity.Name, savedEntity.Name);
        Assert.Equal(entity.Description, savedEntity.Description);

    }

    [Fact]
    public async Task GetAllAsync_ShouldIncludeFromOtherEnitites()
    {
        var status = new StatusEntity { Id = 1, Name = "Active" };
        var employee = new EmployeesEntity { 
            Id = 1, 
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

        // Lägg till projekt
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

        // ACT – Hämta alla projekt
        var result = await repository.GetAllAsync();

        // ASSERT – Kontrollera att data inkluderades korrekt
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

}
