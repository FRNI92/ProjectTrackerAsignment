using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.MenuDialogs;

public class CustomerMenuDialogs
{
    private readonly ICustomerService _customerService;

    public CustomerMenuDialogs(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task ShowCustomerMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Show All Customer");
            Console.WriteLine("2. Create a new Customer");
            Console.WriteLine("4. Delete a Customer");
            Console.WriteLine("0. Go Back To Main Menu");

            Console.Write("\nChoose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ShowCustomerAsync();
                    break;
                case "2":
                    await CreateCustomerAsync();
                    break;
                case "3":
                    await UpdateCustomerAsync();
                    break;
                case "4":
                    await DeleteCustomerAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }


    private async Task ShowCustomerAsync()
    {
        var customers = await _customerService.GetAllCustomerAsync();
        if (customers is Result<IEnumerable<CustomerDto>> customerResult)
        {
            var customersData = customerResult.Data;
            foreach (var customer in customersData)
            {
                Console.WriteLine($"Customer Name: {customer.Name} ");

            }
        }
        else
        {
            Result.Error("Error when loading customers");
        }
    }

    private async Task CreateCustomerAsync()
    {

        var newCustomer = new CustomerDto();
        Console.WriteLine("put in the new Name");
        newCustomer.Name = Console.ReadLine();
        Console.WriteLine("put in the new Email");
        newCustomer.Email = Console.ReadLine();
        Console.WriteLine("put in the new Phonenumber");
        newCustomer.PhoneNumber = Console.ReadLine();


        var result = await _customerService.CreateCustomerAsync(newCustomer);
        if (result is Result<CustomerDto> customerResult)
        {
            Console.WriteLine($"new customer was added {customerResult.Data.Name}");
        }
        else
            {
                Console.WriteLine("We have a problem with customer");
            }
    }

    private async Task UpdateCustomerAsync()
    {
        Console.Clear();
        Console.WriteLine("This Is The Customer List:");

        // Hämta alla kunder
        var customers = await _customerService.GetAllCustomerAsync();

        if (customers is Result<IEnumerable<CustomerDto>> customersResult && customers.Success)
        {
            var customersData = customersResult.Data;

            if (!customersData.Any())
            {
                Console.WriteLine("No Customers found.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }


            // Visa alla kunder
            int index = 1;
            foreach (var customer in customersData)
            {
                Console.WriteLine($"{index}.Customer Name: {customer.Name}\t Email: {customer.Email}\n Phonenumber: {customer.PhoneNumber}");
                Console.WriteLine("------------------------------------");
                index++;
            }

            Console.WriteLine("----Which Customer Would You Like To Update? Enter the number:");

            // Låt användaren välja en kund
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= customersData.Count())
            {
                var selectedCustomer = customersData.ElementAt(choice - 1); //hämta vald kund

                Console.WriteLine($"Enter new name (leave blank to keep current: {selectedCustomer.Name}):");
                var newName = Console.ReadLine();

                Console.WriteLine($"Enter new Email (leave blank to keep current: {selectedCustomer.Email}):");
                var newEmail = Console.ReadLine();

                Console.WriteLine($"Enter new Phonenumber (leave blank to keep current: {selectedCustomer.PhoneNumber}):");
                var newPhonenumber = Console.ReadLine();

                var updatedCustomer = new CustomerDto()
                {
                    Id = selectedCustomer.Id,
                    Name = string.IsNullOrWhiteSpace(newName) ? selectedCustomer.Name : newName,//tom eller null behåll gamla : annar newName
                    Email = string.IsNullOrWhiteSpace(newEmail) ? selectedCustomer.Email : newEmail,
                    PhoneNumber = string.IsNullOrWhiteSpace(newPhonenumber) ? selectedCustomer.PhoneNumber : newPhonenumber
                };
                var result = await _customerService.UpdateCustomerAsync(updatedCustomer);
                if (result is Result<CustomerDto> updateResult && updateResult.Success)
                {
                    Console.WriteLine("Customer updated successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to update the customer.");
                }
            }
        }
    }

    private  static async Task DeleteCustomerAsync()
    {

    }
}
