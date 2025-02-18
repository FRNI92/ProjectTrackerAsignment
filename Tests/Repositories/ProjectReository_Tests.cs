using Data_Infrastructure.Contexts;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Data_Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Tests.Repositories;

public class ProjectReository_Tests
{
    private readonly DataContext _context;
    private readonly IProjectRepository _projectRepository;

    public ProjectReository_Tests()

    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase($"{Guid.NewGuid().ToString()}")
            .Options;

        _context = new DataContext(options);
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
}
