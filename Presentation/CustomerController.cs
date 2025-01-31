using Business.Dtos;
using Business.Interfaces;
using Business.Services;
using Microsoft.AspNetCore.Mvc;


namespace Presentation;


[Route("api/Employees")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly EmployeeService _employeeService;

    public CustomerController(EmployeeService employeeservice)
    {
        _employeeService = employeeservice;
    }

    //public CustomerController(ICustomerService customerService)
    //{
    //    _customerService = customerService;
    //}
    //[HttpGet]
    //public async Task<IActionResult> GetAll()
    //{
    //    var customers = await _customerService.GetAllAsync();
    //    return Ok(customers);
    //}

    [HttpPost]
    public async Task<IActionResult> Create(EmployeeDto employeeDto)
    {
        var employee = await _employeeService.CreateEmployeeAsync(employeeDto);
        return Ok(employee);
    }
    //[HttpPut("{id}")]
    //public async Task<IActionResult> Update(int id, [FromBody] CustomerDto customerDto)
    //{
    //    var updatedCustomer = await _customerService.UpdateCustomerAsync(customerDto);
    //    if (updatedCustomer == null)
    //        return NotFound();

    //    return Ok(updatedCustomer);
    //}
    //[HttpDelete("{id}")]
    //public async Task<IActionResult> Delete(int id)
    //{
    //    var customerDto = new CustomerDto { Id = id };
    //    bool deleted = await _customerService.DeleteCustomerAsync(customerDto);

    //    if (!deleted)
    //        return NotFound();

    //    return NoContent();
    //}
}
