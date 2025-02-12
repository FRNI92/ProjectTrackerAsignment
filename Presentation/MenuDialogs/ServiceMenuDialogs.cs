using Business.Dtos;
using Business.Models;
using Business.Services;

namespace Presentation.MenuDialogs;

public class ServiceMenuDialogs
{
    private readonly ServiceService _serviceService;

    public ServiceMenuDialogs(ServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    private readonly StatusService _statusservice;

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
        if (services is Result<IEnumerable<StatusDto>> servicesResult)
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
                Console.WriteLine("------------------------------------");
                index++;
            }

            Console.WriteLine("----Which Service Would You Like To Update? Enter the number:");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= servicesData.Count())
            {
                var selectedService = servicesData.ElementAt(choice - 1); //hämta vald kund

                Console.WriteLine($"Enter new service name (leave blank to keep current: {selectedService.Name}):");
                var newServiceName = Console.ReadLine();


                var updatedService = new ServiceDto()
                {
                    Id = selectedService.Id,
                    Name = string.IsNullOrWhiteSpace(newServiceName) ? selectedService.Name : newServiceName//tom eller null behåll gamla : annar newName
                };
                var result = await _serviceService.UpdateServiceAsync(updatedService);
                if (result.Success)  // Kontrollera om resultatet var framgångsrikt
                {
                    Console.WriteLine("Status updated successfully!");
                }
                else
                {
                    Console.WriteLine($"Failed to update the Status: {result.ErrorMessage}");
                }
            }
        }
    }

    private async Task DeleteServiceAsync()
    {
        Console.Clear();
        Console.WriteLine("Statuses-MANAGER");
        Console.WriteLine("\tDelete Status");

        Console.WriteLine("This Is The Status-List:");
        var statusesResult = await _statusservice.ReadStatusAsync();
        if (statusesResult is not Result<IEnumerable<StatusDto>> statusResult || !statusResult.Success)
        {
            Console.WriteLine("Failed to load roles or no roles available.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        var statuses = statusResult.Data.ToList();
        if (!statuses.Any())
        {
            Console.WriteLine("No status found.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        // Visa alla kunder
        int index = 1;
        foreach (var status in statuses)
        {
            Console.WriteLine($"{index}. Status-Name: {status.Name}");
            index++;
        }

        Console.WriteLine("----Which Status Would You Like To Delete?");
        if (!int.TryParse(Console.ReadLine(), out int choice) || choice <= 0 || choice > statuses.Count())
        {
            Console.WriteLine("Invalid choice.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        var selectedStatus = statuses.ElementAt(choice - 1);

        Console.WriteLine($"Are you sure you want to delete this Status: {selectedStatus.Name}? (y/n)");
        if (Console.ReadLine()?.ToLower() != "y")
        {
            Console.WriteLine("Status deletion cancelled.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        try
        {
            await _statusservice.DeleteStatusAsync(selectedStatus.Id);
            Console.WriteLine("Role deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete the customer. Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}

