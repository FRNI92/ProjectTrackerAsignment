using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Presentation.Interfaces;

namespace Presentation.MenuDialogs;

public class ServiceMenuDialogs : IServiceMenuDialogs
{
    private readonly IServiceService _serviceService;

    public ServiceMenuDialogs(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    public async Task ShowServicesMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Show All Services");
            Console.WriteLine("2. Create a new Service");
            Console.WriteLine("3. See Details And Update Service");
            Console.WriteLine("4. Delete a Service");
            Console.WriteLine("0. Go Back To Main Menu");

            Console.Write("\nChoose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ShowServiceAsync();
                    break;
                case "2":
                    await CreateServiceAsync();
                    break;
                case "3":
                    await UpdateServiceAsync();
                    break;
                case "4":
                    await DeleteServiceAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }


    private async Task ShowServiceAsync()
    {
        var services = await _serviceService.ReadServiceAsync();
        if (services is Result<IEnumerable<ServiceDto>> servicesResult)
        {
            var servicesData = servicesResult.Data;
            foreach (var service in servicesData)
            {
                Console.WriteLine($"Service Name: {service.Name} ");
            }
        }
        else
        {
            Result.Error("Error when loading customers");
        }
        Console.ReadKey();
    }


    private async Task CreateServiceAsync()
    {
        var newService = new ServiceDto();

        Console.Write("Enter Service name: ");
        newService.Name = Console.ReadLine()!;

        Console.Write("Enter Service name: ");
        newService.Description = Console.ReadLine()!;

        while (true)
        {
            Console.Write("Enter Service duration (hours): ");
            if (decimal.TryParse(Console.ReadLine(), out decimal duration) && duration > 0)
            {
                newService.Duration = duration;
                break;
            }
            Console.WriteLine("Invalid input. Please enter a valid number for duration.");
        }

        while (true)
        {
            Console.Write("Enter Service price: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal price) && price > 0)
            {
                newService.Price = price;
                break;
            }
            Console.WriteLine("Invalid input. Please enter a valid number for price.");
        }

        var createdNewService = await _serviceService.CreateServiceAsync(newService);
        if (createdNewService.Success)
        {
            Console.WriteLine($"Service created: {newService.Name}");
        }
        else
        {
            Console.WriteLine($"Error: {createdNewService.ErrorMessage}");
        }
        Console.ReadKey();
    }

    private async Task UpdateServiceAsync()
    {
        Console.Clear();
        Console.WriteLine("This Is The Service-List:");

        // Hämta alla kunder
        var services = await _serviceService.ReadServiceAsync();

        if (services is Result<IEnumerable<ServiceDto>> servicesResult && servicesResult.Success)
        {
            var servicesData = servicesResult.Data;

            if (!servicesData.Any())
            {
                Console.WriteLine("No Services found.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            int index = 1;
            foreach (var serviceData in servicesData!)
            {
                Console.WriteLine($"{index}.Service Name: {serviceData.Name}\t");
                Console.WriteLine($"Description: {serviceData.Description}\t");
                Console.WriteLine($"Duration: {serviceData.Duration} Hours\t");
                Console.WriteLine($"Price: {serviceData.Price} SEK\t");
                Console.WriteLine("------------------------------------");
                index++;
            }

            Console.WriteLine("----Which Service Would You Like To Update? Enter the number:");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= servicesData.Count())
            {
                var selectedService = servicesData.ElementAt(choice - 1); 

                Console.WriteLine($"Enter new service name (leave blank to keep current: {selectedService.Name}):");
                var newServiceName = Console.ReadLine();


                var updatedService = new ServiceDto()
                {
                    Id = selectedService.Id,
                    Name = string.IsNullOrWhiteSpace(newServiceName) ? selectedService.Name : newServiceName
                };

                Console.WriteLine("\nDo you want to save changes?");
                Console.Write("Type 'yes' to save, or anything else to cancel: ");
                var confirmSave = Console.ReadLine();

                if (confirmSave?.ToLower() != "yes")
                {
                    Console.WriteLine("\nEdit cancelled. Returning to menu...");
                    Console.ReadKey();
                    return;
                }

                var result = await _serviceService.UpdateServiceAsync(updatedService);
                if (result.Success) 
                {
                    Console.WriteLine("Status updated successfully!");
                }
                else
                {
                    Console.WriteLine($"Failed to update the Status: {result.ErrorMessage}");
                }
            }
            Console.ReadKey();
        }
    }

    private async Task DeleteServiceAsync()
    {
        Console.Clear();
        Console.WriteLine("Service-MANAGER");
        Console.WriteLine("\tDelete Service");

        Console.WriteLine("This Is The Service-List:");
        var servicesResult = await _serviceService.ReadServiceAsync();
        if (servicesResult is not Result<IEnumerable<ServiceDto>> serviceResult || !serviceResult.Success)
        {
            Console.WriteLine("Failed to load services or no Servicec available.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        var services = serviceResult.Data.ToList();
        if (!services.Any())
        {
            Console.WriteLine("No service found.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        // Visa alla kunder
        int index = 1;
        foreach (var service in services)
        {
            Console.WriteLine($"{index}. Service-Name: {service.Name}");
            index++;
        }

        Console.WriteLine("----Which Service Would You Like To Delete?");
        if (!int.TryParse(Console.ReadLine(), out int choice) || choice <= 0 || choice > services.Count())
        {
            Console.WriteLine("Invalid choice.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        var selectedService = services.ElementAt(choice - 1);

        Console.WriteLine($"Are you sure you want to delete this Service: {selectedService.Name}? (y/n)");
        if (Console.ReadLine()?.ToLower() != "y")
        {
            Console.WriteLine("Service deletion cancelled.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        try
        {
            await _serviceService.DeleteServiceAsync(selectedService.Id);
            Console.WriteLine("Service deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete the customer. Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}

