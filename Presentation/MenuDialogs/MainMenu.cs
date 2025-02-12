namespace Presentation.MenuDialogs;

public class MainMenu
{
    private readonly ProjectMenuDialogs _projectMenuDialogs;
    private readonly CustomerMenuDialogs _customerMenuDialogs;
    public MainMenu(ProjectMenuDialogs projectMenuDialogs, CustomerMenuDialogs customerMenuDialogs)
    {
        _projectMenuDialogs = projectMenuDialogs;
        _customerMenuDialogs = customerMenuDialogs;
    }

    public async Task ShowMainMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Manage Projects");
            Console.WriteLine("2. Manage Customers");
            Console.WriteLine("3. Manage Employees");
            //Console.WriteLine("4. Manage Roles");ska läggas till
            //Console.WriteLine("5. Manage Servicec");
            //Console.WriteLine("6. Manage Statuses");

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
                    await _EmployeeMenuDialogs.ShowEmployeeMenu();
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

