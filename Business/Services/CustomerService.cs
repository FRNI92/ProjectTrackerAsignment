using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Business.Services;

public class CustomerService : BaseService<CustomerEntity>, ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    public CustomerService(ICustomerRepository customerRepository) : base(customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        Console.WriteLine("CustomerService: Calling repository to fetch all customers...");
        var customer = await _customerRepository.GetAllAsync();
        Console.WriteLine("CustomerService: Mapping entities to DTOs...");
        return customer.Select(customer => CustomerFactory.ToDto(customer)).ToList();

    }
    public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto)
    {
        var entity = CustomerFactory.ToEntity(customerDto); // Mappa DTO till Entity
        var createdEntity = await _customerRepository.CreateAsync(entity); // Skapa entitet
        return CustomerFactory.ToDto(createdEntity); // Mappa tillbaka till DTO
    }

    public async Task<CustomerDto> UpdateCustomerAsync(CustomerDto customerDto)
    {
        var customer = await _customerRepository.GetAsync(x => x.Id == customerDto.Id);
        if (customer == null)
        {
            throw new KeyNotFoundException("Customer not found");
        }

        // Uppdatera fälten
        customer.Name = string.IsNullOrWhiteSpace(customerDto.Name) ? customer.Name : customerDto.Name;
        customer.Email = string.IsNullOrWhiteSpace(customerDto.Email) ? customer.Email : customerDto.Email;
        customer.PhoneNumber = string.IsNullOrWhiteSpace(customerDto.PhoneNumber) ? customer.PhoneNumber : customerDto.PhoneNumber;

        // Uppdatera i databasen
        var updatedCustomer = await _customerRepository.UpdateAsync(x => x.Id == customer.Id, customer);

        // Returnera den uppdaterade kunden som DTO
        return CustomerFactory.ToDto(updatedCustomer);
    }

    public async Task<bool> DeleteCustomerAsync(CustomerDto customerDto)
    {
        var customer = await _customerRepository.GetAsync(x => x.Id == customerDto.Id);
        if (customer == null)
        {
            return false; // Returnera false om kunden inte hittades
        }

        await _customerRepository.DeleteAsync(x => x.Id == customer.Id);
        return true; // Returnera true om raderingen lyckades
    }
}



