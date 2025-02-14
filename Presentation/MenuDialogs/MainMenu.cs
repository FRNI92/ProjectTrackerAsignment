using Presentation.Interfaces;

namespace Presentation.MenuDialogs;

public class MainMenu : IMainMenu
{
    private readonly IProjectMenuDialogs _projectMenuDialogs;
    private readonly ICustomerMenuDialogs _customerMenuDialogs;
    private readonly IEmployeeMenuDialogs _employeeMenuDialogs;
    private readonly IRoleMenuDialog _roleMenuDialog;
    private readonly IStatusMenuDialogs _statusMenuDialogs;
    private readonly IServiceMenuDialogs _serviceMenuDialogs;
    public MainMenu(IProjectMenuDialogs projectMenuDialogs, ICustomerMenuDialogs customerMenuDialogs, IEmployeeMenuDialogs employeeMenuDialogs, IRoleMenuDialog roleMenuDialog, IStatusMenuDialogs statusMenuDialogs, IServiceMenuDialogs serviceMenuDialogs)
    {
        _projectMenuDialogs = projectMenuDialogs;
        _customerMenuDialogs = customerMenuDialogs;
        _employeeMenuDialogs = employeeMenuDialogs;
        _roleMenuDialog = roleMenuDialog;
        _statusMenuDialogs = statusMenuDialogs;
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
                    await _statusMenuDialogs.ShowStatusMenu();
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

