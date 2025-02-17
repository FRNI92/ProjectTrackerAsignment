using Business.Dtos;
using Data_Infrastructure.Entities;
using System.Net.NetworkInformation;

namespace Business.Factories;

public static class StatusFactory
{
    public static StatusDto ToDto (StatusEntity entity)
    {
        return new StatusDto
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }

    public static StatusEntity ToEntity(StatusDto dto)
    {
        return new StatusEntity
        {
            Id = dto.Id,
            Name = dto.Name,
        };
    }

}
