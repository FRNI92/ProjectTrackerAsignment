using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace Tests.Mock_Service_Tests;

public class ServiceService_Tests
{
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly IServiceService _serviceService;

    public ServiceService_Tests()
    {
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _serviceService = new ServiceService(_serviceRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateServiceAsync_ShouldCreateService_AndReturnIResult()
    {
        //arrange
        var newDto = new ServiceDto
        {
            Id = 1,
            Name = "TEST IT ",
            Description = " TESTS IT",
            Duration = 1,
            Price = 100
        };

        _serviceRepositoryMock
            .Setup(repo => repo.DoesEntityExistAsync(It.IsAny<Expression<Func<ServiceEntity, bool>>>()))
            .ReturnsAsync(false);
        _serviceRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<ServiceEntity>()))
            .ReturnsAsync(true);
        _serviceRepositoryMock
            .Setup(repo => repo.SaveAsync())
            .ReturnsAsync(1);
        
        //act
        var result = await _serviceService.CreateServiceAsync(newDto);
        
        //assert 

        Assert.NotNull(result);
        Assert.True(result.Success);
        _serviceRepositoryMock.Verify(repo => repo.DoesEntityExistAsync(It.IsAny<Expression<Func<ServiceEntity, bool>>>()), Times.Once);
        _serviceRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ServiceEntity>()), Times.Once);
        _serviceRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task ReadServiceAsync_ShouleReturnAllServices()
    {
        var serviceList = new List<ServiceEntity>
        {
            new ServiceEntity {Id = 1, Name = "IT"},
            new ServiceEntity {Id = 2, Name = "Consulting"}
        };
        _serviceRepositoryMock
            .Setup(repos => repos.GetAllAsync())
            .ReturnsAsync(serviceList);


        //act
        var result = await _serviceService.ReadServiceAsync();
        //assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result); 

        if (result is Result<IEnumerable<ServiceDto>> successResult)
        {
            var data = successResult.Data.ToList();

            Assert.Equal(2, data.Count); 
            Assert.Equal("IT", data[0].Name);
            Assert.Equal("Consulting", data[1].Name);
        }
        else
        {
            Assert.Fail("Expected Result<IEnumerable<ServiceDto>>, but got something else.");
        }

        _serviceRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ReadServiceByIdAsync_ShouldReuturnBasedOnId()
    {
        //arrange
        var newEntity = new ServiceEntity
        {
            Id = 1,
            Name = "IT"
        };

        _serviceRepositoryMock
            .Setup(repos => repos.GetAsync(It.IsAny<Expression<Func<ServiceEntity, bool>>>()))
            .ReturnsAsync(newEntity);


        //act
        var result = await _serviceService.ReadServiceByIdAsync(newEntity.Id);
        //assert get
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result);

        if (result is Result<ServiceDto> successResult)
        {
            Assert.NotNull(successResult.Data);
            Assert.Equal(newEntity.Id, successResult.Data.Id);
            Assert.Equal(newEntity.Name, successResult.Data.Name);
        }
        else
        {
            Assert.Fail("Expected Result<ServiceDto>, but got something else.");
        }

        _serviceRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<Expression<Func<ServiceEntity, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task UpdateServiceAsync_ShouldUpdate_AndReturnIResult()
    {
        //arrange
        var originalEntity = new ServiceEntity
        {
            Id = 1,
            Name = "Original",
            Description = "IT STUFF"
        };
        var newDto = new ServiceDto
        {
            Id = 1,
            Name = "NEW",
            Description = "IT STUFF"
        };
        var newEntity = new ServiceEntity
        {
            Id = 1,
            Name = "NEW",
            Description = "IT STUFF"
        };
        _serviceRepositoryMock
            .Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ServiceEntity, bool>>>()))
            .ReturnsAsync(originalEntity);

        _serviceRepositoryMock
             .Setup(repos => repos.TransactionUpdateAsync(It.IsAny<Expression<Func<ServiceEntity, bool>>>(),
                It.IsAny<ServiceEntity>()))
             .ReturnsAsync(newEntity);

        _serviceRepositoryMock
            .Setup(repo => repo.SaveAsync())
            .ReturnsAsync(1);
        
        //act
        var result = await _serviceService.UpdateServiceAsync(newDto);
        //assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result);

        _serviceRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<Expression<Func<ServiceEntity, bool>>>()), Times.Once);
        _serviceRepositoryMock.Verify(repo => repo.TransactionUpdateAsync(It.IsAny<Expression<Func<ServiceEntity, bool>>>(), It.IsAny<ServiceEntity>()), Times.Once);
        _serviceRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);

        if (result is Result<ServiceDto> successResult)
        {
            Assert.NotNull(successResult.Data);
            Assert.Equal("NEW", successResult.Data.Name); // Kontrollera att namnet ändrades
            Assert.Equal("IT STUFF", successResult.Data.Description);
        }
        else
        {
            Assert.Fail("Expected Result<ServiceDto>, but got something else.");
        }
    }


    [Fact]
    public async Task DeleteServiceEntity_ShouldRemoveEntity_AndReturnIResult()
    {
        var serviceEntity = new ServiceEntity
        {
            Id = 1,
            Name = "EXISTS",
            Description = "IT STUFF"
            
        };
        var serviceDto = new ServiceDto
        {
            Id = 1,
            Name = "EXISTS",
            Description = "IT STUFF"
        };

        _serviceRepositoryMock
            .Setup(repo => repo
            .DoesEntityExistAsync(It.IsAny<Expression<Func<ServiceEntity, bool>>>()))
            .ReturnsAsync(true);

        _serviceRepositoryMock
            .Setup(repo => repo.RemoveAsync(It.IsAny<Expression<Func<ServiceEntity, bool>>>()))
            .ReturnsAsync(true);
        _serviceRepositoryMock
            .Setup(repo => repo.SaveAsync())
            .ReturnsAsync(1);
        //act
        var result = await _serviceService.DeleteServiceAsync(serviceDto.Id);
        //assert does remove save
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IResult>(result);
        _serviceRepositoryMock.Verify(repo => repo.DoesEntityExistAsync(It.IsAny<Expression<Func<ServiceEntity, bool>>>()), Times.Once);
        _serviceRepositoryMock.Verify(repo => repo.RemoveAsync(It.IsAny<Expression<Func<ServiceEntity, bool>>>()), Times.Once);
        _serviceRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
    }
}




//    Task<IResult> DeleteServiceEntity(int id); test is done
//    Task<IResult> UpdateServiceAsync(ServiceDto serviceDto); test is done
//    Task<IResult> ReadServiceByIdAsync(int serviceId); test is done
//  Task<IResult> CreateServiceAsync(ServiceDto serviceDto); test klart
//    Task<IResult> ReadServiceAsync(); test done

