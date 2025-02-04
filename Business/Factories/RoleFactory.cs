using Business.Dtos;
using Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Business.Factories;

public static class RoleFactory
{
    public static RolesEntity ToEntity(RolesDto rolesDto)
    {
        return new RolesEntity
        {
            Name = rolesDto.Name,
            Description = rolesDto.Description,
        };
    }

        public static RolesDto ToDto(RolesEntity rolesEntity)
    {
        return new RolesDto
        {
            Name = rolesEntity.Name,
            Description = rolesEntity.Description,
        };
    }

    public static void UpdateEntity(RolesEntity entity, RolesDto dto) 
    {
        // Viktigt: ÄNDRA INTE entity.Id
        entity.Name = dto.Name;
        entity.Description = dto.Description;
    }
}

    

