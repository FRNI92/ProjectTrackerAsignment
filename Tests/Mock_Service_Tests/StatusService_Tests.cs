using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Moq;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace Tests.Mock_Service_Tests;

public class StatusService_Tests
{
    private readonly Mock<IStatusRepository> _statusRepositoryMock;
    private readonly IStatusService _statusService;

    public StatusService_Tests()
    {
        _statusRepositoryMock = new Mock <IStatusRepository>();
        _statusService = new StatusService(_statusRepositoryMock.Object);// Cant instansiate Interfaces
    }

    [Fact]
    public async Task CreateStatusAsync_ShouldCreateStatus_AndReturnIresult()
    {

        //arrange
        var statusDto = new StatusDto
        {
            Id = 1,
            Name = "TestName"
        };

        _statusRepositoryMock
            .Setup(repo => repo.DoesEntityExistAsync(It.IsAny<Expression<Func<StatusEntity, bool>>>()))
            .ReturnsAsync(false);
        _statusRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<StatusEntity>()))
            .ReturnsAsync(true);
        _statusRepositoryMock
            .Setup(repo => repo.SaveAsync())
            .ReturnsAsync(1);
        //act
        var result = await _statusService.CreateStatusAsync(statusDto);
        //assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        _statusRepositoryMock.Verify(repo => repo.DoesEntityExistAsync(It.IsAny<Expression<Func<StatusEntity, bool>>>()), Times.Once);
        _statusRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<StatusEntity>()), Times.Once);
        _statusRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task ReadAllAsync_ShouldReturnAllStatus_AndIResult()
    {
        //arrange
        var statusList = new List<StatusEntity>
        {
            new StatusEntity {Id = 1, Name = "Active"},
            new StatusEntity {Id = 2, Name = "SuperActive"}
        };
        _statusRepositoryMock
            .Setup(repos => repos.GetAllAsync())
            .ReturnsAsync(statusList);


        //act
        var result = await _statusService.ReadStatusAsync();
        //assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result); // Kontrollera att vi får tillbaka IResult

        if (result is Result<IEnumerable<StatusDto>> successResult)
        {
            var data = successResult.Data.ToList();

            Assert.Equal(2, data.Count); // Kontrollera att vi fick tillbaka 2 statusar
            Assert.Equal("Active", data[0].Name);
            Assert.Equal("SuperActive", data[1].Name);
        }
        else
        {
            Assert.Fail("Expected Result<IEnumerable<StatusDto>>, but got something else.");
        }

        _statusRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact] 
    public async Task UpdateStatusAsync_ShouldUpdateEntity_AndReturnIResult()
    {
        //arrange
        var originalEntity = new StatusEntity
        {
            Id = 1,
            Name = "Original"
        };
        var newDto = new StatusDto
        {
            Id = 1,
            Name = "NEW"
        };
        var newEntity = new StatusEntity
        {
            Id = 1,
            Name = "NEW"
        };
        _statusRepositoryMock
            .Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<StatusEntity, bool>>>()))
            .ReturnsAsync(originalEntity);

        _statusRepositoryMock
             .Setup(repos => repos.TransactionUpdateAsync(It.IsAny<Expression<Func<StatusEntity, bool>>>(),
                It.IsAny<StatusEntity>()))
             .ReturnsAsync(newEntity);

        _statusRepositoryMock
            .Setup(repo => repo.SaveAsync())
            .ReturnsAsync(1);
        //act
        var result = await _statusService.UpdateStatusAsync(newDto);
        //assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result);

        _statusRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<Expression<Func<StatusEntity, bool>>>()), Times.Once);
        _statusRepositoryMock.Verify(repo => repo.TransactionUpdateAsync(It.IsAny<Expression<Func<StatusEntity, bool>>>(), It.IsAny<StatusEntity>()), Times.Once);
        _statusRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);

        if (result is Result<StatusDto> successResult)
        {
            Assert.NotNull(successResult.Data);
            Assert.Equal("NEW", successResult.Data.Name); // Kontrollera att namnet ändrades
        }
        else
        {
            Assert.Fail("Expected Result<StatusDto>, but got something else.");
        }
    }

}




//    Task<IResult> CreateStatusAsync(StatusDto statusDto); test done
//    Task<IResult> ReadStatusAsync(); test done
//    Task<IResult> UpdateStatusAsync(StatusDto statusDto); test done
//    Task<IResult> DeleteStatusAsync(int id);