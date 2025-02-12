using Business.Dtos;
using Data.Entities;

namespace Business.Factories;

public static class EmployeeFactory
{
    public static EmployeesEntity ToEntity(EmployeeDto employeeDto)
    {
        return new EmployeesEntity
        {
            Id = employeeDto.Id,
            FirstName = employeeDto.FirstName,
            LastName = employeeDto.LastName,
            Email = employeeDto.Email,
            RoleId = employeeDto.RoleId,
        };
    }

    public static EmployeeDto ToDto(EmployeesEntity employeesEntity)
    {
        return new EmployeeDto
        {
            Id = employeesEntity.Id,
            FirstName = employeesEntity.FirstName,
            LastName = employeesEntity.LastName,
            Email = employeesEntity.Email,
            RoleId = employeesEntity.RoleId,
            RoleName = employeesEntity.Role.Name,
        };
    }
}
