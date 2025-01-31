using Business.Dtos;
using Business.Factories;
using Data.Interfaces;

namespace Business.Services;

public class EmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    //Create
    public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employeeDto)
    {
        try 
        { 
            if (employeeDto == null)
            throw new ArgumentNullException(nameof(employeeDto), "Employee data cannot be null.");

            bool exists = await _employeeRepository.DoesEntityExistAsync(e => e.Email == employeeDto.Email);
            if (exists)
                throw new InvalidOperationException("An employee with this email already exists.");

            var newEmployeeEntity = EmployeeFactory.ToEntity(employeeDto);
            var CreatedEmployee = await _employeeRepository.CreateAsync(newEmployeeEntity);

            return EmployeeFactory.ToDto(CreatedEmployee);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error With CreateEmployeeAsync{ex.Message}");
            throw;
        }
    }

    //Read

    //Update

    //Delete
}
