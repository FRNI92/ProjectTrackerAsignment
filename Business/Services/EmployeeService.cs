using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
using Data.Repositories;
using System.Diagnostics;

namespace Business.Services;

public class EmployeeService
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

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        try
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Select(EmployeeFactory.ToDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was an error getting employees{ex.Message}");
            throw;
        }
    }

    public async Task<EmployeeDto> GetEmployeeByIdAsync(int id)
    {
        try
        {
            var employee = await _employeeRepository.GetAsync(e => e.Id == id);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found.");

            return EmployeeFactory.ToDto(employee);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occured when getting employee{ex.Message}");
            throw;
        }
    }


    public async Task<EmployeeDto> UpdateEmployeeAsync(int id, EmployeeDto updatedEmployeeDto)
    {
        try
        {
            var updatedEntity = EmployeeFactory.ToEntity(updatedEmployeeDto);

            var updatedEmployee = await _employeeRepository.UpdateAsync(e => e.Id == id, updatedEntity);
            if (updatedEmployee == null)
                throw new KeyNotFoundException("Employee not found for update.");

            return EmployeeFactory.ToDto(updatedEmployee);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occured when updating employee{ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteEmployeeByIdAsync(int id)
    {
        {
            try
            {
                var exists = await _employeeRepository.DoesEntityExistAsync(e => e.Id == id);

                if (!exists)
                {
                    Console.WriteLine("Employee hittades inte.");
                    return false;
                }
                return await _employeeRepository.DeleteAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured when deleting{ex.Message}");
                throw;
            }
        }
    }
}




