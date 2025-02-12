using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Interfaces;
using System.Diagnostics.Eventing.Reader;

namespace Presentation.MenuDialogs;

public class EmployeeMenuDialogs
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
                    await UpdateEmployeetAsync();
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

        var newEmployee= new EmployeeDto();

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



 }




