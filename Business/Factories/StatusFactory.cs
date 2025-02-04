using Business.Dtos;
using Data.Entities;
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

    public static void UpdatedEntity(StatusEntity entity, StatusDto dto)
    {
        entity.Name = dto.Name;
    }
}
