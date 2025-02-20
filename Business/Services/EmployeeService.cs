using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data_Infrastructure.Interfaces;
using Data_Infrastructure.Repositories;
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
        if (employeeDto == null)
        {
            return Result.BadRequest("Employee Dto was not filled in correcly");
        }

        await _employeeRepository.BeginTransactionAsync();

        try
        {
            bool exists = await _employeeRepository.DoesEntityExistAsync(e => e.Email == employeeDto.Email);
            if (exists)
            {
                await _employeeRepository.RollBackTransactionAsync();
                return Result.AlreadyExists("An employee with this email already exists.");
            }
            var newEmployeeEntity = EmployeeFactory.ToEntity(employeeDto);
            var createdEmployee = await _employeeRepository.AddAsync(newEmployeeEntity);
            if (createdEmployee == false)
            {
                await _employeeRepository.RollBackTransactionAsync();
                return Result.BadRequest("Could not save the employee");
            }

            var isSaved = await _employeeRepository.SaveAsync();
            if (isSaved > 0)
            {
                await _employeeRepository.CommitTransactionAsync();

                var savedEmployee = await _employeeRepository.GetAsync(e => e.Id == newEmployeeEntity.Id);

                var employeeDtoWithRole = EmployeeFactory.ToDto(savedEmployee);
                return Result<EmployeeDto>.OK(employeeDtoWithRole);
            }

            await _employeeRepository.RollBackTransactionAsync();
            return Result.BadRequest("Could not save the employee");
        }

        catch (Exception ex)
        {
            await _employeeRepository.RollBackTransactionAsync();
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
        if (updatedEmployeeDto == null)
        {
            return Result.BadRequest("Dto was not corecctly filed");
        }

        await _employeeRepository.BeginTransactionAsync();
        try
        {
            var existingEmployee = await _employeeRepository.GetAsync(e => e.Id == id);
            if (existingEmployee == null)
            {
                await _employeeRepository.RollBackTransactionAsync();
                return Result.NotFound("Employee not found for update.");
            }

            var updatedEntity = EmployeeFactory.ToEntity(updatedEmployeeDto);


            var updatedEmployee = await _employeeRepository.TransactionUpdateAsync(e => e.Id == id, updatedEntity);
            if (updatedEmployee == null)
            {
                await _employeeRepository.RollBackTransactionAsync();
                return Result.Error("Failed to update employee.");
            }


            var isUpdatedAndSaved = await _employeeRepository.SaveAsync();
            if (isUpdatedAndSaved > 0)
            {
                await _employeeRepository.CommitTransactionAsync();
                var updateEmployeeDto = EmployeeFactory.ToDto(updatedEmployee);
                return Result<EmployeeDto>.OK(updateEmployeeDto);
            }

            await _employeeRepository.RollBackTransactionAsync();
            return Result.Error("Update failed, no changes were saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error occured when updating employee{ex.Message}{ex.StackTrace}");
            return Result.Error("There was en error when updating employee");
        }
    }

    public async Task<IResult> DeleteEmployeeByIdAsync(int id)
    {

        await _employeeRepository.BeginTransactionAsync();
        try
        {
            if (!await _employeeRepository.DoesEntityExistAsync(e => e.Id == id))
            {
                await _employeeRepository.RollBackTransactionAsync();
                return Result.NotFound("Could not find the employee to delete.");
            }

            var deleteSuccess = await _employeeRepository.RemoveAsync(e => e.Id == id);
            if (!deleteSuccess)
            {
                await _employeeRepository.RollBackTransactionAsync();
                return Result.Error("Failed to delete employee.");
            }

            var isSaved = await _employeeRepository.SaveAsync();
            if (isSaved > 0)
            {
                await _employeeRepository.CommitTransactionAsync();
                return Result.OK();
            }

            await _employeeRepository.RollBackTransactionAsync();
            return Result.Error("Could not save deletion.");
        }
        catch (Exception ex)
        {
            await _employeeRepository.RollBackTransactionAsync();
            Debug.WriteLine($"An error occured when deleting{ex.Message}{ex.StackTrace}");
            return Result.Error("There was an error when deleting the Employee");
        }       
    }
}




