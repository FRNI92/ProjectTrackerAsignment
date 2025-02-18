using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Data_Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories;

public class EmployeeRepository_Tests : Test_Base
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeRepository_Tests()
    {
        _employeeRepository = new EmployeeRepository(_context); ;
    }

    [Fact] 
    public async Task GetAllAsync_ShouldIncludeRoleData()
    {
        
        
        // Arrange
        var role = new RolesEntity
        {
            Id = 1,
            Description = "testing stuff",
            Name = "Test Role"
        };

        var employee = new EmployeesEntity
        {
            Id = 1,
            FirstName = "Fredrik",
            LastName = "Nilsson",
            Email = "Fredrik@domain.com",
            RoleId = 1
        };

        _context.Roles.Add(role);
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        //Act
        var result = await _employeeRepository.GetAllAsync();

        //Assert

        Assert.NotNull(result);
        Assert.Single(result);

        var fetchedEmployee = result.First();
        Assert.NotNull(fetchedEmployee.Role);
        Assert.Equal("Test Role", fetchedEmployee.Role.Name);

    }
}
