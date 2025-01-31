using Business.Dtos;
using Data.Entities;

namespace Business.Factories;

public static class ProjectFactory
{
    
    /// <summary>
    /// Takes a variable of type ProjectDto and moves values over to ProjectEntity
    /// ToEntity-method takes type ProjectDto "name of choice" and returns ProjectEntity
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public static ProjectEntity ToEntity(ProjectDto projectDto)
         
    {
        return new ProjectEntity
        {
            Id = projectDto.Id,
            ProjectNumber = projectDto.ProjectNumber,
            Name = projectDto.Name,
            Description = projectDto.Description,
            StartDate = projectDto.StartDate,
            EndDate = projectDto.EndDate,
            StatusId = projectDto.StatusId,
            EmployeeId = projectDto.EmployeeId,
            CustomerId = projectDto.CustomerId,
            ServiceId = projectDto.ServiceId,       
            
        };
    }

    /// <summary>
    /// Takes a variable of type ProjectEntity and moves values over to ProjectDto
    /// ToDto-method takes a projectEntity(can be any name) of the type ProjectEntity
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public static ProjectDto ToDto(ProjectEntity projectEntity)
    {
        return new ProjectDto
        {
            Id = projectEntity.Id,
            ProjectNumber = projectEntity.ProjectNumber,
            Name = projectEntity.Name,
            Description = projectEntity.Description,
            StartDate = projectEntity.StartDate,
            EndDate = projectEntity.EndDate,
            StatusId = projectEntity.StatusId,
            StatusName = projectEntity.Status?.Name ?? "No status assigned",// hantera null under utveckling
            
            CustomerId = projectEntity.CustomerId,
            CustomerName = projectEntity.Customer?.Name ?? "No customer",
            
            EmployeeId = projectEntity.EmployeeId,
            EmployeeName = projectEntity.Employee?.FirstName ?? "No employee",
            
            ServiceId = projectEntity.ServiceId,
            ServiceName = projectEntity.Service?.Name ?? "No service",
        };
    }
}
