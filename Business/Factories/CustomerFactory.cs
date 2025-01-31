using Business.Dtos;
using Data.Entities;

namespace Business.Factories;

public static class CustomerFactory
{/// <summary>
/// converts enty to dto and returns customerDto. when Reading
/// </summary>
/// <param name="entity"></param>
/// <returns></returns>
    public static CustomerDto ToDto(CustomerEntity entity)
    {
        return new CustomerDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber
        };
    }
    /// <summary>
    /// convert from dto to entity and returns customerEnity. when creating or updating
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public static CustomerEntity ToEntity(CustomerDto dto)
    {
        return new CustomerEntity
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };
    }
}
