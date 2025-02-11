using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Business.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IResult> CreateCustomerAsync(CustomerDto customerDto)
    {
        try
        {
            var entity = CustomerFactory.ToEntity(customerDto); // Mappa DTO till Entity
            var result = await _customerRepository.CreateAsync(entity); // Skapa entitet
            if (result) // Om skapandet lyckades
            {
                return Result.OK();
            }
            return Result.Error("Unable to create customer");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return Result.Error($"Error when creating customer: {ex.Message} {ex.StackTrace}");
        }
    }

    public async Task<IResult> GetAllCustomerAsync()
    {
        try
        {
            var customers = await _customerRepository.GetAllAsync();
            if (customers != null && customers.Any())
            {
                var customerDtoList = customers.Select(customer => CustomerFactory.ToDto(customer)).ToList(); // Använd ToList() här
                return Result<IEnumerable<CustomerDto>>.OK(customerDtoList);
            }
            else
            {
                return Result.Error("Customer was empty") ;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Something went wrong when Getting Customers{ex.Message} {ex.StackTrace}");
            return Result.Error("Something went wrong when Getting Customers");
        }
    }


    public async Task<IResult> UpdateCustomerAsync(CustomerDto customerDto)
    {
        try
        {
            var customer = await _customerRepository.GetAsync(x => x.Id == customerDto.Id);
            if (customer == null)
            {
                return Result.NotFound("Customer not found");
            }

            // Uppdatera fälten
            customer.Name = string.IsNullOrWhiteSpace(customerDto.Name) ? customer.Name : customerDto.Name;
            customer.Email = string.IsNullOrWhiteSpace(customerDto.Email) ? customer.Email : customerDto.Email;
            customer.PhoneNumber = string.IsNullOrWhiteSpace(customerDto.PhoneNumber) ? customer.PhoneNumber : customerDto.PhoneNumber;

            // Uppdatera i databasen
            var updatedCustomer = await _customerRepository.UpdateAsync(x => x.Id == customer.Id, customer);
            var updatedCustomerDto = CustomerFactory.ToDto(updatedCustomer);

            // Returnera den uppdaterade kunden som DTO
            return Result<CustomerDto>.OK(updatedCustomerDto);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Something went wrong when updating customer {ex.Message} {ex.StackTrace}");
            return Result.Error("Something went wrong when updating customer");
        }
    }

    public async Task<IResult> DeleteCustomerAsync(CustomerDto customerDto)
    {
        try
        {
            var customer = await _customerRepository.GetAsync(x => x.Id == customerDto.Id);
            if (customer == null)
            {
                return Result.NotFound("Could not find customer to delete");
            }

            await _customerRepository.DeleteAsync(x => x.Id == customer.Id);
            return Result.OK();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Could not delete the customer correctly {ex.Message} {ex.StackTrace}");
            return Result.BadRequest("Could not delete the customer correctly ");
        }
    }
}



