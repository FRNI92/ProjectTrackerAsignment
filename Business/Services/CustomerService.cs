using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Data_Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Xml.XPath;

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

        await _customerRepository.BeginTransactionAsync();
        try
        {
            var entity = CustomerFactory.ToEntity(customerDto); // Mappa DTO till Entity
            var addResult = await _customerRepository.AddAsync(entity); // Skapa entitet
            if (!addResult) // Om skapandet lyckades
            {
                await _customerRepository.RollBackTransactionAsync();
                return Result.Error("Unable to create customer");              
            }

            var saveAddResult = await _customerRepository.SaveAsync();
            if(saveAddResult > 0)
            {
                await _customerRepository.CommitTransactionAsync();
                return Result.OK();
            }
            await _customerRepository.RollBackTransactionAsync();
            return Result.Error("Unable to create customer");
        }
        catch (Exception ex)
        {
            await _customerRepository.RollBackTransactionAsync();
            Debug.WriteLine($" {ex.Message} {ex.StackTrace}");
            return Result.Error($"Error when creating customer:");
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
        await _customerRepository.BeginTransactionAsync();
        try
        {
            var customer = await _customerRepository.GetAsync(x => x.Id == customerDto.Id);
            if (customer == null)
            {
                await _customerRepository.RollBackTransactionAsync();
                return Result.NotFound("Customer not found");
            }

            // Uppdatera fälten
            customer.Name = string.IsNullOrWhiteSpace(customerDto.Name) ? customer.Name : customerDto.Name;
            customer.Email = string.IsNullOrWhiteSpace(customerDto.Email) ? customer.Email : customerDto.Email;
            customer.PhoneNumber = string.IsNullOrWhiteSpace(customerDto.PhoneNumber) ? customer.PhoneNumber : customerDto.PhoneNumber;

            // Uppdatera i databasen

            var updatedCustomer = await _customerRepository.TransactionUpdateAsync(x => x.Id == customer.Id, customer);
            if (updatedCustomer == null)
            {
                await _customerRepository.RollBackTransactionAsync();
                return Result.Error("There was an error when updating customer");
            }
            

            // Returnera den uppdaterade kunden som DTO
            var isSavedCustomer = await _customerRepository.SaveAsync();
            if (isSavedCustomer > 0)
            {
                await _customerRepository.CommitTransactionAsync();

                var updatedCustomerDto = CustomerFactory.ToDto(updatedCustomer);
                return Result<CustomerDto>.OK(updatedCustomerDto);
            }

            await _customerRepository.RollBackTransactionAsync();
            return Result.Error("Something went wrong when updating customer");
        }
        catch (Exception ex)
        {
            await _customerRepository.RollBackTransactionAsync();
            Debug.WriteLine($"Something went wrong when updating customer {ex.Message} {ex.StackTrace}");
            return Result.Error("Something went wrong when updating customer");
        }
    }

    public async Task<IResult> DeleteCustomerAsync(CustomerDto customerDto)
    {
        await _customerRepository.BeginTransactionAsync();
        try
        {
            var customer = await _customerRepository.GetAsync(x => x.Id == customerDto.Id);
            if (customer == null)
            {
                await _customerRepository.RollBackTransactionAsync();
                return Result.NotFound("Could not find customer to delete");
            }

            var isRemoved = await _customerRepository.RemoveAsync(x => x.Id == customer.Id);
            if (!isRemoved)
            {
                await _customerRepository.RollBackTransactionAsync();
                return Result.Error("There was an error when removing customer");
            }
            
            var isSaved = await _customerRepository.SaveAsync();
            if(isSaved > 0)
            {
                await _customerRepository.CommitTransactionAsync();
                return Result.OK();
            }

        await _customerRepository.RollBackTransactionAsync();
        return Result.Error("There was an error when saving customer");

        }
        catch (Exception ex)
        {
            await _customerRepository.RollBackTransactionAsync();
            Debug.WriteLine($"Could not delete the customer correctly {ex.Message} {ex.StackTrace}");
            return Result.BadRequest("Could not delete the customer correctly ");
        }
    }
}



