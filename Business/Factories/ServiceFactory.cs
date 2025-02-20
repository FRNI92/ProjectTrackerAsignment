using Business.Dtos;
using Business.Interfaces;
using Data_Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Identity.Client;
namespace Business.Factories;

public static class ServiceFactory
{
    public static ServiceDto ToDto(ServiceEntity entity)
    {
        return new ServiceDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Duration = entity.Duration,
            Price = entity.Price,
        };
    }

    public static ServiceEntity ToEntity(ServiceDto dto)
    {
        return new ServiceEntity
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Duration = dto.Duration,
            Price = dto.Price,
        };
    }
}
