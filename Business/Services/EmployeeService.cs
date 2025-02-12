using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
using Data.Repositories;
using System.Diagnostics;

namespace Business.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }
    public async Task<IResult> CreateEmployeeAsync(EmployeeDto employeeDto)
    {
        try
        {
            if (employeeDto == null)
            {
                Result.BadRequest("Employee Dto was not filled in correcly");
            }

            bool exists = await _employeeRepository.DoesEntityExistAsync(e => e.Email == employeeDto.Email);
            if (exists)
            {
                return Result.AlreadyExists("An employee with this email already exists.");
            }
            else
            {
                var newEmployeeEntity = EmployeeFactory.ToEntity(employeeDto);
                var CreatedEmployee = await _employeeRepository.CreateAsync(newEmployeeEntity);
                return Result.OK();
            }
        }

        catch (Exception ex)
        {
            Debug.WriteLine($"Error With CreateEmployeeAsync{ex.Message}{ex.StackTrace}");
            return Result.Error("Something went wrong when creating Employee");
        }
    }

    public async Task<IResult> GetEmployeeAsync()
    {
        try
        {
            var allEmployees = await _employeeRepository.GetAllAsync();
            if (allEmployees.Any())
            {
                var employeeDto = allEmployees.Select(EmployeeFactory.ToDto).ToList();
                return Result<IEnumerable<EmployeeDto>>.OK(employeeDto);
            }
            else
            {
                return Result.NotFound("Could not find any employees");
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"There was an error getting employees{ex.Message}{ex.StackTrace}");
            return Result.Error($"There was an error when getting Employees{ex.Message}");
        }
    }

    public async Task<IResult> GetEmployeeByIdAsync(int id)
    {
        try
        {
            var employee = await _employeeRepository.GetAsync(e => e.Id == id);
            if (employee == null)
            {
                return Result.NotFound("Could not find any Employees with that ID:");
            }
            else
            {
                var employeedto = EmployeeFactory.ToDto(employee);
                return Result<EmployeeDto>.OK(employeedto);
            }    

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error occured when getting employee{ex.Message}{ex.StackTrace}");
            return Result.Error("There was an error getting Employee with ID:");
        }
    }


    public async Task<IResult> UpdateEmployeeAsync(int id, EmployeeDto updatedEmployeeDto)
    {
        try
        {
            if(updatedEmployeeDto == null)
            {
                return Result.BadRequest("Dto was not corecctly filed");
            }
            else
            {
            var updatedEntity = EmployeeFactory.ToEntity(updatedEmployeeDto);
            var updatedEmployee = await _employeeRepository.UpdateAsync(e => e.Id == id, updatedEntity);
                if (updatedEmployee == null)
                {
                    return Result.NotFound("Employee not found for update.");
                }
                else
                {
                var updateEmployeeDto = EmployeeFactory.ToDto(updatedEmployee);
                return Result<EmployeeDto>.OK(updateEmployeeDto);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error occured when updating employee{ex.Message}{ex.StackTrace}");
            return Result.Error("There was en error when updating employee");
        }
    }

    public async Task<IResult> DeleteEmployeeByIdAsync(int id)
    {      
        try
        {
            var exists = await _employeeRepository.DoesEntityExistAsync(e => e.Id == id);

            if (!exists)
            {
                return Result.NotFound("Could not find the employee to Delete");
            }
            else
            {
                var deleteSuccess = await _employeeRepository.DeleteAsync(e => e.Id == id);
                if (deleteSuccess)
                {
                    return Result.OK(); // Return OK if deletion was successful
                }
                else
                {
                    return Result.Error("Failed to Delete Employee by ID:");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occured when deleting{ex.Message}{ex.StackTrace}");
            return Result.Error("There was an error when deleting the Employee");
        }       
    }
}




