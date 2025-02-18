using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Presentation.Interfaces;

namespace Presentation.MenuDialogs;

public class CustomerMenuDialogs : ICustomerMenuDialogs
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
            Console.WriteLine("3. See Details And Update Customer");
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
        Console.ReadKey();
    }


    private async Task CreateCustomerAsync()
    {

        var newCustomer = new CustomerDto();

        Console.Write("Enter Customer name: ");
        newCustomer.Name = Console.ReadLine()!;

        Console.Write("Enter customer email: ");
        newCustomer.Email = Console.ReadLine()!;

        Console.Write("Enter customer phonenumber: ");
        newCustomer.PhoneNumber = Console.ReadLine()!;

        var createdNewCustomer = await _customerService.CreateCustomerAsync(newCustomer);
        if (createdNewCustomer.Success)
        {
            Console.WriteLine($"Customer created: {newCustomer.Name} {newCustomer.Email}");
        }
        else
        {
            Console.WriteLine($"Error: {createdNewCustomer.ErrorMessage}");
        }

        Console.ReadKey();
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

                Console.WriteLine("\nDo you want to save changes?");
                Console.Write("Type 'yes' to save, or anything else to cancel: ");
                var confirmSave = Console.ReadLine();

                if (confirmSave?.ToLower() != "yes")// går in i blocket om det kommer in något annat än yes
                {
                    Console.WriteLine("\nEdit cancelled. Returning to menu...");
                    Console.ReadKey();
                    return;
                }

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

    private async Task DeleteCustomerAsync()
    {
        Console.Clear();
        Console.WriteLine("CUSTOMER-MANAGER");
        Console.WriteLine("\tDelete Customer");

        Console.WriteLine("This Is The Customer List:");
        var customersResult = await _customerService.GetAllCustomerAsync();
        if (customersResult is not Result<IEnumerable<CustomerDto>> customerResult || !customerResult.Success)
        {
            Console.WriteLine("Failed to load customers or no customers available.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        var customers = customerResult.Data.ToList();
        if (!customers.Any())
        {
            Console.WriteLine("No customers found.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        // Visa alla kunder
        int index = 1;
        foreach (var customer in customers)
        {
            Console.WriteLine($"{index}. Name: {customer.Name}, Email: {customer.Email}");
            index++;
        }

        Console.WriteLine("----Which Customer Would You Like To Delete?");
        if (!int.TryParse(Console.ReadLine(), out int choice) || choice <= 0 || choice > customers.Count())
        {
            Console.WriteLine("Invalid choice.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        var selectedCustomer = customers.ElementAt(choice - 1);

        // Be om bekräftelse innan radering
        Console.WriteLine($"Are you sure you want to delete this customer: {selectedCustomer.Name}? (y/n)");
        if (Console.ReadLine()?.ToLower() != "y")
        {
            Console.WriteLine("Customer deletion cancelled.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        // Försök att radera kunden
        try
        {
            await _customerService.DeleteCustomerAsync(selectedCustomer);
            Console.WriteLine("Customer deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete the customer. Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}

