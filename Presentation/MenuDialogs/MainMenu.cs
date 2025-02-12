﻿namespace Presentation.MenuDialogs;

public class MainMenu
{
    private readonly ProjectMenuDialogs _projectMenuDialogs;
    private readonly CustomerMenuDialogs _customerMenuDialogs;
    private readonly EmployeeMenuDialogs _employeeMenuDialogs;
    private readonly RoleMenuDialog _roleMenuDialog;
    private readonly StatusMenuDiallogs _statusMenuDiallogs;
    private readonly ServiceMenuDialogs _serviceMenuDialogs;
    public MainMenu(ProjectMenuDialogs projectMenuDialogs, CustomerMenuDialogs customerMenuDialogs, EmployeeMenuDialogs employeeMenuDialogs, RoleMenuDialog roleMenuDialog, StatusMenuDiallogs statusMenuDiallogs, ServiceMenuDialogs serviceMenuDialogs)
    {
        _projectMenuDialogs = projectMenuDialogs;
        _customerMenuDialogs = customerMenuDialogs;
        _employeeMenuDialogs = employeeMenuDialogs;
        _roleMenuDialog = roleMenuDialog;
        _statusMenuDiallogs = statusMenuDiallogs;
        _serviceMenuDialogs = serviceMenuDialogs;
    }

    public async Task ShowMainMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Manage Projects");
            Console.WriteLine("2. Manage Customers");
            Console.WriteLine("3. Manage Employees");
            Console.WriteLine("4. Manage Roles");
            Console.WriteLine("5. Manage Statuses");
            Console.WriteLine("6. Manage Services");

            Console.WriteLine("0. Exit");

            Console.Write("\nChoose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await _projectMenuDialogs.ShowProjectsMenuAsync();
                    break;

                case "2":
                    await _customerMenuDialogs.ShowCustomerMenu();
                    break;

                case "3":
                    await _employeeMenuDialogs.ShowEmployeeMenuAsync();
                    break;

                case "4":
                    await _roleMenuDialog.ShowRolesMenu();
                    break;

                case "5":
                    await _statusMenuDiallogs.ShowStatusMenu();
                    break;

                case "6":
                    await _serviceMenuDialogs.ShowServicesMenu();
                    break;
                case "0":
                        return;
                    default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }
}

