using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Moq;

namespace Tests.Mock_Service_Tests;

public class ProjectService_Tests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;//mocka repos
    private readonly Mock<IServiceService> _serviceServiceMock; //mocka serviceservice
    private readonly ProjectService _projectService; // Använd den riktiga servicen

    public ProjectService_Tests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>(); // Mocka repository
        _serviceServiceMock = new Mock<IServiceService>(); // Mocka ServiceService
        _projectService = new ProjectService(_projectRepositoryMock.Object, _serviceServiceMock.Object); // Skicka in mockade objekt
    }

    [Fact]
    public async Task CreateProjectAsync_ShouldReturnIResult()
    {
        // Arrange
        var mockServiceDto = new ServiceDto
        {
            Id = 1,
            Name = "Test Service",
            Price = 100 // projectservice needs serviceService to calc the price
        };
        //needs this setup to since I inject and use serviceService in projectService
        _serviceServiceMock
            .Setup(service => service.ReadServiceByIdAsync(It.IsAny<int>()))//to enable the return och mockservicedto
            .ReturnsAsync(Result<ServiceDto>.OK(mockServiceDto));

        var projectDto = new ProjectDto { Id = 1, ProjectNumber = "testnumber", Name = "Test Project", ServiceId = 1 };      

        _projectRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<ProjectEntity>())).ReturnsAsync(true);
        _projectRepositoryMock.Setup(repo => repo.SaveAsync()).ReturnsAsync(1); // Simulates success save

        // Act
        var result = await _projectService.CreateProjectAsync(projectDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result);
        _projectRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ProjectEntity>()), Times.Once);

    }

    [Fact]
    public async Task GetallProjectAsync_ShouldReturnIResult_AndAllProjects()
    {
        var testProjects = new List<ProjectEntity>
    {
        new ProjectEntity { Id = 1, ProjectNumber = "testnumber1", Name = "testname1" },
        new ProjectEntity { Id = 2, ProjectNumber = "testnumber2", Name = "testname2" }
    };

        _projectRepositoryMock
            .Setup(repos => repos.GetAllAsync())
            .ReturnsAsync(testProjects); // Mockar att repo returnerar en lista med projekt
        
        // Act
        var result = await _projectService.GetAllProjectAsync(); // Vänta på Task<IResult>

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result);

        if (result is Result<IEnumerable<ProjectDto>> successResult)
        {
            var projectDtos = successResult.Data.ToList();
            
            Assert.Equal(2, projectDtos.Count); // Kontrollera att vi får 2 projekt
            Assert.Equal("testnumber1", projectDtos[0].ProjectNumber);
            Assert.Equal("testnumber2", projectDtos[1].ProjectNumber);
        }
        else
        {
            Assert.Fail("Expected Result<List<ProjectDto>>, but got something else.");
        }
    }
}




    //Task<IResult> CreateProjectAsync(ProjectDto projectDto);
    //Task<IResult> GetAllProjectAsync();
    //Task<IResult> UpdateProjectAsync(ProjectDto projectDto);
    //Task<IResult> DeleteProjectAsync(ProjectDto projectDto);