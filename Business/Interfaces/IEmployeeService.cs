using Business.Dtos;

namespace Business.Interfaces;

public interface IEmployeeService
{
    public Task<IResult> CreateEmployeeAsync(EmployeeDto employeeDto);
    public Task<IResult> GetEmployeeAsync();
    public Task<IResult> GetEmployeeByIdAsync(int id);

    public Task<IResult> UpdateEmployeeAsync(int id, EmployeeDto updatedEmployeeDto);
    public Task<IResult> DeleteEmployeeByIdAsync(int id);
}
