using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Moq;
using System.Linq.Expressions;
using Xunit.Sdk;

namespace Tests.Mock_Service_Tests;

public class ProjectService_Tests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IServiceService> _serviceServiceMock; 
    private readonly IProjectService _projectService; 

    public ProjectService_Tests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>(); 
        _serviceServiceMock = new Mock<IServiceService>(); 
        _projectService = new ProjectService(_projectRepositoryMock.Object, _serviceServiceMock.Object); 
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
        _projectRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);

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
            .ReturnsAsync(testProjects);

        // Act
        var result = await _projectService.GetAllProjectAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result);

        _projectRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);// getting all projects. getAsync takes expression

        if (result is Result<IEnumerable<ProjectDto>> successResult)
        {
            var projectDtos = successResult.Data.ToList();

            Assert.Equal(2, projectDtos.Count);
            Assert.Equal("testnumber1", projectDtos[0].ProjectNumber);
            Assert.Equal("testnumber2", projectDtos[1].ProjectNumber);
        }
        else
        {
            Assert.Fail("Expected Result<List<ProjectDto>>, but got something else.");
        }
    }

    [Fact]
    public async Task UpdateProjectAsync_ShouldReturnIResult_WithUpdatedDto()
    {
        //arrange
        var original = new ProjectEntity
        {
            Id = 1,
            ProjectNumber = "Testnumber",
            Name = "OriginalName",
            ServiceId = 1
        };
        var updatedEntity = new ProjectEntity
        {
            Id = 1,
            ProjectNumber = "Testnumber",
            Name = "ChangedName",
            ServiceId = 1
        };
        var updatedDto = new ProjectDto
        {
            Id = 1,
            ProjectNumber = "Testnumber",
            Name = "ChangedName",
            ServiceId = 1
        };
        _projectRepositoryMock
        .Setup(repos => repos.GetAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()))
        .ReturnsAsync(original);

        _projectRepositoryMock
         .Setup(repos => repos.TransactionUpdateAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>(), It.IsAny<ProjectEntity>()))
         .ReturnsAsync(updatedEntity);

        _projectRepositoryMock
            .Setup(repos => repos.SaveAsync())
            .ReturnsAsync(1);

        // Act 
        var result = await _projectService.UpdateProjectAsync(updatedDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result);

        _projectRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()), Times.Once);
        _projectRepositoryMock.Verify(repo => repo.TransactionUpdateAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>(), It.IsAny<ProjectEntity>()), Times.Once);
        _projectRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);

        if (result is Result<ProjectDto> updatedProjectDto)
        {
            Assert.NotNull(updatedProjectDto.Data);
            Assert.Equal("ChangedName", updatedProjectDto.Data.Name); 
        }
        else
        {
            Assert.Fail("Expected Result<ProjectDto>, but got something else.");
        }

    }


    [Fact]
    public async Task DeleteProjectAsync_ShouldRemoveEntity_AndReturnIResult()
    {
        //arrange
        var projectDto = new ProjectDto
        {
            Id = 1,
            ProjectNumber = "TestNumber",
            Name = "TestProject"
        };

        var existingProject = new ProjectEntity
        {
            Id = 1,
            ProjectNumber = "TestNumber",
            Name = "TestProject"
        };


        _projectRepositoryMock
            .Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()))
            .ReturnsAsync(existingProject);

        _projectRepositoryMock
            .Setup(repo => repo.RemoveAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()))
            .ReturnsAsync(true);

        _projectRepositoryMock
            .Setup(repo => repo.SaveAsync())
            .ReturnsAsync(1);

        //act
        var result = await _projectService.DeleteProjectAsync(projectDto);


        //assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result);
        Assert.True(result.Success);


        _projectRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()), Times.Once);
        _projectRepositoryMock.Verify(repo => repo.RemoveAsync(It.IsAny<Expression<Func<ProjectEntity, bool>>>()), Times.Once);
        _projectRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
        //might not need to verify since the test needs the data in setup to work. but its called verify so...

    }
}
    //Task<IResult> CreateProjectAsync(ProjectDto projectDto); test done
    //Task<IResult> GetAllProjectAsync(); test done
    //Task<IResult> UpdateProjectAsync(ProjectDto projectDto); test done
    //Task<IResult> DeleteProjectAsync(ProjectDto projectDto); test done






