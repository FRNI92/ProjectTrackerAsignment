using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Moq;
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

}




//    Task<IResult> CreateStatusAsync(StatusDto statusDto);
//    Task<IResult> DeleteStatusAsync(int id);
//    Task<IResult> ReadStatusAsync();
//    Task<IResult> UpdateStatusAsync(StatusDto statusDto);