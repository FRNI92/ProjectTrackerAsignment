using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace Tests.Mock_Service_Tests;

public class RoleService_Tests
{
    private readonly Mock<IRolesRepository> _rolesRepositoryMock;
    private readonly IRoleService _roleService;

    public RoleService_Tests()
    {
        _rolesRepositoryMock = new Mock<IRolesRepository>();
        _roleService = new RoleService(_rolesRepositoryMock.Object);
    }


    [Fact]
    public async Task CreateRolesAsync_ShouldSaveToDatabase_AndReturnIResult()
    {
        //arrange
        var newDto = new RolesDto
        {
            Id = 1,
            Name = "Admin",
            Description = "ALLPOWER",
            
        };

        _rolesRepositoryMock
                .Setup(repo => repo.DoesEntityExistAsync(It.IsAny<Expression<Func<RolesEntity, bool>>>()))
                .ReturnsAsync(false);
        _rolesRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<RolesEntity>()))
                .ReturnsAsync(true);
        _rolesRepositoryMock
                .Setup(repo => repo.SaveAsync())
                .ReturnsAsync(1);
    
       
        //act
        var result = await _roleService.CreateRoleAsync(newDto);
        
        
        //assert

        Assert.NotNull(result);
        Assert.True(result.Success);
        _rolesRepositoryMock.Verify(repo => repo.DoesEntityExistAsync(It.IsAny<Expression<Func<RolesEntity, bool>>>()), Times.Once);
        _rolesRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<RolesEntity>()), Times.Once);
        _rolesRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);

    }

    [Fact]
    public async Task GetAllRolesAsync_ShouldGetAllRoles_AndReturnIResult()
    {
        //arrange
        var roleList = new List<RolesEntity>
        {
            new RolesEntity {Id = 1, Description = "Does stuff"},
            new RolesEntity {Id = 2, Description = "Does more stuff"}
        };

        _rolesRepositoryMock
            .Setup(repos => repos.GetAllAsync())
            .ReturnsAsync(roleList);
        //act
        var result = await _roleService.GetAllRolesAsync();
     

        //assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result); 

        if (result is Result<IEnumerable<RolesDto>> successResult)
        {
            var data = successResult.Data.ToList();

            Assert.Equal(2, data.Count); 
            Assert.Equal("Does stuff", data[0].Description);
            Assert.Equal("Does more stuff", data[1].Description);
        }
        else
        {
            Assert.Fail("Expected Result<IEnumerable<RolesDto>>, but got something else.");
        }

        _rolesRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateRolesAsync_ShouldUpdate_AndReturnIResult()
    {
        //arrange
        var originalEntity = new RolesEntity
        {
            Id = 1,
            Name = "Original",
            Description = "DOES STUFF"
        };
        var newDto = new RolesDto
        {
            Id = 1,
            Name = "NEW",
            Description = "DOES STUFF"
        };
        var newEntity = new RolesEntity
        {
            Id = 1,
            Name = "NEW",
            Description = "DOES STUFF"
        };
        _rolesRepositoryMock
            .Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<RolesEntity, bool>>>()))
            .ReturnsAsync(originalEntity);

        _rolesRepositoryMock
             .Setup(repos => repos.TransactionUpdateAsync(It.IsAny<Expression<Func<RolesEntity, bool>>>(),It.IsAny<RolesEntity>()))
             .ReturnsAsync(newEntity);

        _rolesRepositoryMock
            .Setup(repo => repo.SaveAsync())
            .ReturnsAsync(1);
        
        //act

        var result = await _roleService.UpdateRolesAsync(newDto);
        
        //assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result);

        _rolesRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<Expression<Func<RolesEntity, bool>>>()), Times.Once);
        _rolesRepositoryMock.Verify(repo => repo.TransactionUpdateAsync(It.IsAny<Expression<Func<RolesEntity, bool>>>(), It.IsAny<RolesEntity>()), Times.Once);
        _rolesRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);

        if (result is Result<RolesDto> successResult)
        {
            Assert.NotNull(successResult.Data);
            Assert.Equal("NEW", successResult.Data.Name); 
            Assert.Equal("DOES STUFF", successResult.Data.Description);
        }
        else
        {
            Assert.Fail("Expected Result<RolesDto>, but got something else.");
        }
    }

    [Fact]
    public async Task DeleteRolesAsync_ShouldRemoveAndReturnIResult()
    {
        //arrange
        var newDto = new RolesDto
        {
            Id = 1,
            Name = "SUPERDUPER",
            Description = "Does Everything"
        };
        _rolesRepositoryMock
            .Setup(repos => repos.DoesEntityExistAsync(It.IsAny<Expression<Func<RolesEntity, bool>>>()))
            .ReturnsAsync(true);

        _rolesRepositoryMock
            .Setup(repos => repos.RemoveAsync(It.IsAny<Expression<Func<RolesEntity, bool>>>()))
            .ReturnsAsync(true);

        _rolesRepositoryMock
            .Setup(repos => repos.SaveAsync())
            .ReturnsAsync(1);
        
        //act
         var result = await _roleService.DeleteRolesAsync(newDto);
        
        //assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result);
        _rolesRepositoryMock.Verify(repo => repo.DoesEntityExistAsync(It.IsAny<Expression<Func<RolesEntity, bool>>>()), Times.Once);
        _rolesRepositoryMock.Verify(repo => repo.RemoveAsync(It.IsAny<Expression<Func<RolesEntity, bool>>>()), Times.Once);
        _rolesRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
    }
}

