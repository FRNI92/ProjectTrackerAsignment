using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Presentation.Interfaces;

namespace Presentation.MenuDialogs;

public class StatusMenuDialogs : IStatusMenuDialogs
{
    private readonly IStatusService _statusservice;

    public StatusMenuDialogs(IStatusService statusservice)
    {
        _statusservice = statusservice;
    }

    public async Task ShowStatusMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Show All Statuses");
            Console.WriteLine("2. Create a new Status");
            Console.WriteLine("3. See Details And Update Status");
            Console.WriteLine("4. Delete a Status");
            Console.WriteLine("0. Go Back To Main Menu");

            Console.Write("\nChoose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ShowStatusAsync();
                    break;
                case "2":
                    await CreateStatusAsync();
                    break;
                case "3":
                    await UpdateStatusAsync();
                    break;
                case "4":
                    await DeleteStatusAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }


    private async Task ShowStatusAsync()
    {
        var statuses = await _statusservice.ReadStatusAsync();
        if (statuses is Result<IEnumerable<StatusDto>> statusResult)
        {
            var statusesData = statusResult.Data;
            foreach (var status in statusesData)
            {
                Console.WriteLine($"Status Name: {status.Name} ");
            }
        }
        else
        {
            Result.Error("Error when loading customers");
        }
        Console.ReadKey();
    }


    private async Task CreateStatusAsync()
    {

        var newStatus = new StatusDto();

        Console.Write("Enter Status name: ");
        newStatus.Name = Console.ReadLine()!;

        var createdNewStatus = await _statusservice.CreateStatusAsync(newStatus);
        if (createdNewStatus.Success)
        {
            Console.WriteLine($"Status created: {newStatus.Name}");
        }
        else
        {
            Console.WriteLine($"Error: {createdNewStatus.ErrorMessage}");
        }
        Console.ReadKey();
    }

    private async Task UpdateStatusAsync()
    {
        Console.Clear();
        Console.WriteLine("This Is The Status-List:");

        // Hämta alla kunder
        var statuses = await _statusservice.ReadStatusAsync();

        if (statuses is Result<IEnumerable<StatusDto>> statusesResult && statusesResult.Success)
        {
            var statusesData = statusesResult.Data;

            if (!statusesData.Any())
            {
                Console.WriteLine("No statuses found.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            int index = 1;
            foreach (var statusdata in statusesData)
            {
                Console.WriteLine($"{index}.Status Name: {statusdata.Name}\t");
                Console.WriteLine("------------------------------------");
                index++;
            }

            Console.WriteLine("----Which Status Would You Like To Update? Enter the number:");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= statusesData.Count())
            {
                var selectedStatus = statusesData.ElementAt(choice - 1); //hämta vald kund

                Console.WriteLine($"Enter new status name (leave blank to keep current: {selectedStatus.Name}):");
                var newStatusName = Console.ReadLine();


                var updatedStatus = new StatusDto()
                {
                    Id = selectedStatus.Id,
                    Name = string.IsNullOrWhiteSpace(newStatusName) ? selectedStatus.Name : newStatusName//tom eller null behåll gamla : annar newName

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

                var result = await _statusservice.UpdateStatusAsync(updatedStatus);
                if (result.Success)  // Kontrollera om resultatet var framgångsrikt
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

    private async Task DeleteStatusAsync()
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
            Console.WriteLine("Status deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete the status. Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}

