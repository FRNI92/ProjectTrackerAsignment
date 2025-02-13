using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Interfaces;
using Presentation.Interfaces;
using System.Diagnostics.Eventing.Reader;

namespace Presentation.MenuDialogs;

public class EmployeeMenuDialogs : IEmployeeMenuDialogs
{
    private readonly IEmployeeService _employeeService;

    public EmployeeMenuDialogs(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task ShowEmployeeMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Show Preview Of All Employees");
            Console.WriteLine("2. Create a new Employee");
            Console.WriteLine("3. Show Detailed View And Update a Employee");
            Console.WriteLine("4. Delete an Employee");
            Console.WriteLine("0. Go Back To Main Menu");

            Console.Write("\nChoose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ShowEmployeePreviewAsync();
                    break;
                case "2":
                    await CreateEmployeeAsync();
                    break;
                case "3":
                    await UpdateEmployeeAsync();
                    break;
                case "4":
                    await DeleteEmployeeAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    private async Task ShowEmployeePreviewAsync()
    {
        Console.WriteLine("This is the list of Employees");
        var result = await _employeeService.GetEmployeeAsync();

        if (result is Result<IEnumerable<EmployeeDto>> employeeResult)
        {
            var employeeList = employeeResult.Data;
            foreach (var employee in employeeList)
            {
                Console.WriteLine($"Employee Name: {employee.FirstName}");
            }
        }
        else
        {
            Console.WriteLine("No projects found.");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }


    private async Task CreateEmployeeAsync()
    {

        var newEmployee = new EmployeeDto();

        Console.Write("Enter Employee first name: ");
        newEmployee.FirstName = Console.ReadLine()!;

        Console.Write("Enter Employee first last name: ");
        newEmployee.LastName = Console.ReadLine()!;

        Console.Write("Enter Employee Email: ");
        newEmployee.Email = Console.ReadLine()!;

        Console.Write("Enter Employee Role ID: ");
        // Försök att konvertera input till int
        int roleId;
        while (!int.TryParse(Console.ReadLine(), out roleId))
        {
            Console.Write("Invalid input. Please enter a valid Role ID: ");
        }

        newEmployee.RoleId = roleId;

        var createdEmployee = await _employeeService.CreateEmployeeAsync(newEmployee);
        if (createdEmployee.Success)
        {
            Console.WriteLine($"Employee created: {newEmployee.FirstName} {newEmployee.LastName}");
        }
        else
        {
            Console.WriteLine($"Error: {createdEmployee.Success}");
        }

        Console.ReadKey();
    }

    private async Task UpdateEmployeeAsync()
    {
        Console.Clear();
        Console.WriteLine("This Is The Employee List:");

        // Hämta alla kunder
        var employees = await _employeeService.GetEmployeeAsync();

        if (employees is Result<IEnumerable<EmployeeDto>> employeeResult && employeeResult.Success)
        {
            var employeesData = employeeResult.Data;

            if (!employeesData.Any())
            {
                Console.WriteLine("No Employees found.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            // Visa alla kunder
            int index = 1;
            foreach (var employee in employeesData)
            {
                Console.WriteLine($"{index}.Employee: {employee.FirstName}\t Name: {employee.LastName}");
                Console.WriteLine($"Current Email: {employee.Email}");
                Console.WriteLine($"Customer Role: {employee.RoleName}");
                Console.WriteLine("------------------------------------");
                index++;
            }

            Console.WriteLine("----Which Employee Would You Like To Update? Enter the number:");

            // Låt användaren välja en kund
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= employeesData.Count())
            {
                var selectedEmployee = employeesData.ElementAt(choice - 1); //hämta vald kund
                Console.Write($"Enter new first name (leave blank to keep current:({selectedEmployee.FirstName}): ");
                var newFirstName = Console.ReadLine();
                Console.Write($"Enter new last name(leave blank to keep current:({selectedEmployee.LastName}): ");
                var newLastName = Console.ReadLine();
                Console.Write($"Enter new Email (leave blank to keep current:({selectedEmployee.Email}): ");
                var newEmail = Console.ReadLine();
                Console.Write($"Enter new Role (leave blank to keep current:({selectedEmployee.RoleId}): ");
                var newRoleId = Console.ReadLine();

                var updatedEmployee = new EmployeeDto
                {
                    Id = selectedEmployee.Id, // Skicka ID:t istället för att söka med namn
                    FirstName = string.IsNullOrWhiteSpace(newFirstName) ? selectedEmployee.FirstName : newFirstName,//tom eller null behåll gamla : annar newName
                    LastName = string.IsNullOrWhiteSpace(newLastName) ? selectedEmployee.LastName : newLastName,
                    Email = string.IsNullOrWhiteSpace(newEmail) ? selectedEmployee.Email : (newEmail),
                    RoleId = string.IsNullOrWhiteSpace(newRoleId) ? selectedEmployee.RoleId : int.Parse(newRoleId)
                };

                await _employeeService.UpdateEmployeeAsync(updatedEmployee.Id, updatedEmployee);
                Console.WriteLine("\nCustomer updated successfully!");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
            }
        }
    }

    private async Task DeleteEmployeeAsync()
    {
        Console.Clear();
        Console.WriteLine("\tEMPLOYEE-MANAGER");
        Console.WriteLine("\tDelete Employee");

        Console.WriteLine("This Is The Employee List:");

        // Hämta alla projekt
        var employeesResult = await _employeeService.GetEmployeeAsync();

        if (employeesResult is Result<IEnumerable<EmployeeDto>> employeeResult && employeeResult.Success)
        {
            var employees = employeeResult.Data.ToList();

            if (!employees.Any())
            {
                Console.WriteLine("No employees found.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            // Visa alla kunder
            int index = 1;
            foreach (var employee in employees)
            {
                Console.WriteLine($"{index}. Name: {employee.FirstName}, Email: {employee.Email}");
                index++;
            }

            Console.WriteLine("----Which Employee Would You Like To Delete?");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= employees.Count())
            {
                var selectedEmployee = employees.ElementAt(choice - 1);

                // Be om bekräftelse innan radering
                Console.WriteLine($"Are you sure you want to delete this employee: {selectedEmployee.FirstName}? (y/n)");
                var confirmation = Console.ReadLine();

                if (confirmation?.ToLower() == "y")
                {
                    Console.WriteLine($"Deleting project: {selectedEmployee.FirstName}");

                    try
                    {
                        await _employeeService.DeleteEmployeeByIdAsync(selectedEmployee.Id);
                        Console.WriteLine("Project deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete the Project. Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Employee deletion cancelled.");
                }
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }
        else
        {
            Console.WriteLine("Failed to load employees or no employees available.");
        }
        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }

}




