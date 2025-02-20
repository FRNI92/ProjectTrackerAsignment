using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Presentation.Interfaces;
using System.Transactions;

namespace Presentation.MenuDialogs;

public class ProjectMenuDialogs : IProjectMenuDialogs
{
    private readonly IProjectService _projectService;

    private readonly IStatusService _statusService;

    private readonly IEmployeeService _employeeService;

    private readonly ICustomerService _customerService;

    private readonly IServiceService _serviceService;

    public ProjectMenuDialogs(IProjectService projectService, IEmployeeService employeeService, IStatusService statusService, ICustomerService customerService, IServiceService serviceService)
    {
        _projectService = projectService;
        _employeeService = employeeService;
        _statusService = statusService;
        _customerService = customerService;
        _serviceService = serviceService;
    }

    public async Task ShowProjectsMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Show Preview Of All Projects");
            Console.WriteLine("2. Create a new Project");
            Console.WriteLine("3. Show Detailed View And Update a Project");
            Console.WriteLine("4. Delete a Project");
            Console.WriteLine("0. Go Back To Main Menu");

            Console.Write("\nChoose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ShowProjectsPreviewAsync();
                    break;
                case "2":
                    await CreateProjectAsync();
                    break;
                case "3":
                    await UpdateProjectAsync();
                    break;
                case "4":
                    await DeleteProjectAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    private async Task ShowProjectsPreviewAsync()
    {
        Console.Clear();
        Console.WriteLine("DEBUG: ShowProjectsPreviewAsync körs");

        var result = await _projectService.GetAllProjectAsync();

        if (result is Result<IEnumerable<ProjectDto>> projectResult)
        {
            var projects = projectResult.Data;
            Console.WriteLine($"DEBUG: Antal projekt i listan: {projects.Count()}");

            foreach (var project in projects)
            {
                Console.WriteLine($"Project ID: {project.Id}");
                Console.WriteLine($"Project Number: {project.ProjectNumber}");
                Console.WriteLine($"Name: {project.Name}");
                Console.WriteLine($"Time Period: {project.StartDate} - {project.EndDate}");
                Console.WriteLine($"Current Status: {project.StatusName}");
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("No projects found.");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }


    private async Task CreateProjectAsync()
    {
        var newProject = new ProjectDto();

        Console.Write("Enter Project Number: ");
        newProject.ProjectNumber = Console.ReadLine()!;

        Console.Write("Enter Project name: ");
        newProject.Name = Console.ReadLine()!;

        Console.Write("Enter Project Description: ");
        newProject.Description = Console.ReadLine();

        DateTime startDate;
        Console.Write("Enter Start Date (YYYY-MM-DD): ");
        while (!DateTime.TryParse(Console.ReadLine(), out startDate))
        {
            Console.WriteLine("Invalid date format. Please try again (YYYY-MM-DD):");
        }
        newProject.StartDate = startDate;

        DateTime endDate;
        Console.Write("Enter End Date (YYYY-MM-DD): ");
        while (!DateTime.TryParse(Console.ReadLine(), out endDate))
        {
            Console.WriteLine("Invalid date format. Please try again (YYYY-MM-DD):");
        }
        newProject.EndDate = endDate;

        
        var statusResult = await _statusService.ReadStatusAsync();
        int selectedStatusId = 1; // Standardstatus ("Not Started")

        if (statusResult is Result<IEnumerable<StatusDto>> statuses && statuses.Success)
        {
            var statusList = statuses.Data.ToList();

            if (statusList.Any())
            {
                Console.WriteLine($"\nSelect Status (Leave blank to use default '{statusList[0].Name}'):");
                for (int i = 0; i < statusList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {statusList[i].Name}");
                }

                while (true)
                {
                    Console.Write("\nEnter the number of the Status: ");
                    var input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine($"Using default Status: {statusList[0].Name}");
                        break;
                    }

                    if (int.TryParse(input, out int choice) && choice > 0 && choice <= statusList.Count)
                    {
                        selectedStatusId = statusList[choice - 1].Id;
                        Console.WriteLine($"Selected Status: {statusList[choice - 1].Name}");
                        break;
                    }

                    Console.WriteLine("Invalid selection. Please enter a valid number.");
                }
            }
        }
        newProject.StatusId = selectedStatusId;


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

            Console.WriteLine("Select Project Manager:");

            for (int i = 0; i < employees.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {employees[i].FirstName} {employees[i].LastName}");
            }

            int selectedEmployeeIndex;
            while (true)
            {
                Console.Write("Enter the number of the Employee: ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out selectedEmployeeIndex) 
                    && selectedEmployeeIndex > 0 
                    && selectedEmployeeIndex <= employees.Count)
                {
                    newProject.EmployeeId = employees[selectedEmployeeIndex - 1].Id;
                    break;
                }

                Console.WriteLine("Invalid selection. Please enter a valid number.");
            }


            var customersResult = await _customerService.GetAllCustomerAsync();
            if (customersResult is Result<IEnumerable<CustomerDto>> customerResult && customerResult.Success)
            {
                var customers = customerResult.Data?.ToList();

                if (!customers.Any())
                {
                    Console.WriteLine("No customers found.");
                    Console.WriteLine("\nPress any key to return to the menu...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("Select customer:");

                for (int i = 0; i < customers.Count; i++)
                {
                    Console.WriteLine($"" +
                        $"{i + 1}. " +
                        $"{customers[i].Name} " +
                        $"{customers[i].Email} " +
                        $"{customers[i].PhoneNumber}");
                }

                int selectedCustomerIndex;
                while (true)
                {
                    Console.Write("Enter the number of the Customer: ");
                    var input = Console.ReadLine();

                    if (int.TryParse(input, out selectedCustomerIndex) 
                        && selectedCustomerIndex > 0 
                        && selectedCustomerIndex <= customers.Count)
                    {
                        newProject.CustomerId = customers[selectedCustomerIndex - 1].Id;
                        break;
                    }

                    Console.WriteLine("Invalid selection. Please enter a valid number.");
                }
            }

            var servicesResult = await _serviceService.ReadServiceAsync();
            if (servicesResult is Result<IEnumerable<ServiceDto>> serviceResult && serviceResult.Success)
            {
                var services = serviceResult.Data?.ToList();

                if (!services.Any())
                {
                    Console.WriteLine("No services found.");
                    Console.WriteLine("\nPress any key to return to the menu...");
                    Console.ReadKey();
                    return;
                }


                Console.WriteLine("Select service:");
                for (int i = 0; i < services.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {services[i].Name} {services[i].Description}");
                }
                int selectedServiceIndex; // is used in next If
                while (true)
                {
                    Console.Write("Enter the number of the Service: ");
                    var input = Console.ReadLine();// now user choose from "list"

                    if (int.TryParse(input, out selectedServiceIndex) && selectedServiceIndex > 0 && selectedServiceIndex <= services.Count)
                    {
                        newProject.ServiceId = services[selectedServiceIndex - 1].Id;
                        break;
                    }
                    Console.WriteLine("Invalid selection. Please enter a valid number.");
                }
            }

            decimal duration;
            while (true)
            {
                Console.Write("Enter the duration (in hours): ");
                var input = Console.ReadLine();

                if (decimal.TryParse(input, out duration) && duration > 0)
                {
                    break;
                }

                Console.WriteLine("Invalid input. Please enter a valid number.");
            }

            newProject.Duration = duration;


            var createdProjectResult = await _projectService.CreateProjectAsync(newProject);

            if (createdProjectResult is Result<ProjectDto> createdProjectData)
            {
                var createdProject = createdProjectData.Data;
                Console.WriteLine($"Project created! Project name: {createdProject.Name} Total Cost: {createdProject.TotalPrice} :-");
            }
            else
            {

                Console.WriteLine($"Error: {createdProjectResult.ErrorMessage}");
            }
        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
        }
    }


    private async Task UpdateProjectAsync()
    {
        Console.Clear();
        Console.WriteLine("This Is The Projects List:");

        var projects = await _projectService.GetAllProjectAsync();

        if (projects is Result<IEnumerable<ProjectDto>> projectResult && projectResult.Success)
        {
            var projectsData = projectResult.Data;

            if (!projectsData.Any())
            {
                Console.WriteLine("No Projects found.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            int index = 1;
            foreach (var project in projectsData)
            {
                Console.WriteLine($"{index}.ProjectNumber: {project.ProjectNumber}\t Name: {project.Name}");
                Console.WriteLine($"Current Status: {project.StatusName}");
                Console.WriteLine($"Customer Name: {project.CustomerName}");
                Console.WriteLine($"Employee Name: {project.EmployeeName}");
                Console.WriteLine($"The Service Purchased: {project.ServiceName}");
                Console.WriteLine($"The Service Duration is set to: {project.Duration} Hours");
                Console.WriteLine($"The Service Total Price Is: {project.TotalPrice} SEK");

                Console.WriteLine("------------------------------------");
                index++;
            }


            Console.WriteLine("----Which Project Would You Like To Update? Enter the number:");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= projectsData.Count())
            {
                var selectedProject = projectsData.ElementAt(choice - 1); 

                Console.Write("Enter new name (leave blank to keep current or type 'exit' to cancel): ");
                var newName = Console.ReadLine();
                if (newName?.ToLower() == "exit")
                {
                    Console.WriteLine("\nEdit cancelled. Returning to menu...");
                    Console.ReadKey();
                    return;
                }

                Console.Write($"Enter new Description (leave blank to keep current:({selectedProject.Description}): ");
                var newDescription = Console.ReadLine();

                Console.Write($"Enter new StartDate (leave blank to keep current:({selectedProject.StartDate}): ");
                var newStartDate = Console.ReadLine();

                Console.Write($"Enter new EndDate (leave blank to keep current:({selectedProject.EndDate}): ");
                var newEndDate = Console.ReadLine();



                var statusResult = await _statusService.ReadStatusAsync();
                int selectedStatusId = 1; // Standardstatus ("Not Started")

                if (statusResult is Result<IEnumerable<StatusDto>> statuses && statuses.Success)
                {
                    var statusList = statuses.Data.ToList();

                    if (statusList.Any())
                    {
                        Console.WriteLine($"\nSelect Status (Leave blank to use default '{statusList[0].Name}'):");
                        for (int i = 0; i < statusList.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {statusList[i].Name}");
                        }

                        while (true)
                        {
                            Console.Write("\nEnter the number of the Status: ");
                            var input = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(input))
                            {
                                Console.WriteLine($"Using default Status: {statusList[0].Name}");
                                break;
                            }

                            if (int.TryParse(input, out int statusChoice) && statusChoice > 0 && statusChoice <= statusList.Count)
                            {
                                selectedStatusId = statusList[statusChoice - 1].Id;
                                Console.WriteLine($"Selected Status: {statusList[choice - 1].Name}");
                                break;
                            }

                            Console.WriteLine("Invalid selection. Please enter a valid number.");
                        }
                    }
                }
                var newStatusId = selectedStatusId;


                int newCustomerId = selectedProject.CustomerId;
                var customersResult = await _customerService.GetAllCustomerAsync();
                if (customersResult is Result<IEnumerable<CustomerDto>> customerResult && customerResult.Success)
                {
                    var customers = customerResult.Data?.ToList();

                    if (!customers.Any())
                    {
                        Console.WriteLine("No customers found.");
                        Console.WriteLine("\nPress any key to return to the menu...");
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine("Select customer:");

                    for (int i = 0; i < customers.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {customers[i].Name} {customers[i].Email}");
                    }

                    int selectedCustomerIndex;
                    while (true)
                    {
                        Console.Write("Enter the number of the Customer: ");
                        var input = Console.ReadLine();

                        if (int.TryParse(input, out selectedCustomerIndex) && selectedCustomerIndex > 0 && selectedCustomerIndex <= customers.Count)
                        {
                            newCustomerId = customers[selectedCustomerIndex - 1].Id;
                            break;
                        }

                        Console.WriteLine("Invalid selection. Please enter a valid number.");
                    }
                }

                var newServiceId = selectedProject.ServiceId; 
                var servicesResult = await _serviceService.ReadServiceAsync();
                if (servicesResult is Result<IEnumerable<ServiceDto>> serviceResult && serviceResult.Success)
                {
                    var services = serviceResult.Data?.ToList();

                    if (!services.Any())
                    {
                        Console.WriteLine("No services found.");
                        Console.WriteLine("\nPress any key to return to the menu...");
                        Console.ReadKey();
                        return;
                    }


                    Console.WriteLine("Select service:");
                    for (int i = 0; i < services.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {services[i].Name} {services[i].Description}");
                    }
                    int selectedServiceIndex;
                    while (true)
                    {
                        Console.Write("Enter the number of the Service: ");
                        var input = Console.ReadLine();


                       
                        if (int.TryParse(input, out selectedServiceIndex) && selectedServiceIndex > 0 && selectedServiceIndex <= services.Count)
                        {
                            newServiceId = services[selectedServiceIndex - 1].Id;
                            break;
                        }
                        Console.WriteLine("Invalid selection. Please enter a valid number.");
                    }
                }

                Console.Write($"Enter new Service-Duration:(leave blank to keep current:({selectedProject.Duration}): ");
                var newServiceDuration = Console.ReadLine();


                var employeesResult = await _employeeService.GetEmployeeAsync();
                int selectedEmployeeId = selectedProject.EmployeeId;
              
                if (employeesResult is Result<IEnumerable<EmployeeDto>> employeeResult && employeeResult.Success)
                {
                    var employees = employeeResult.Data.ToList();

                    if (employees.Any())
                    {
                        Console.WriteLine("\nSelect New Project Manager (Leave blank to keep current):");
                        for (int i = 0; i < employees.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {employees[i].FirstName} {employees[i].LastName}");
                        }

                        while (true)
                        {
                            Console.Write("\nEnter the number of the new Project Manager (or press Enter to keep current): ");
                            var input = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(input))
                            {
                                Console.WriteLine($"Keeping current Project Manager: {selectedProject.EmployeeId}");
                                break;
                            }
                           
                            if (int.TryParse(input, out int employeeChoice) && employeeChoice > 0 && employeeChoice <= employees.Count)
                            {
                                selectedEmployeeId = employees[employeeChoice - 1].Id;
                                Console.WriteLine($"Selected new Project Manager: {employees[employeeChoice - 1].FirstName} {employees[employeeChoice - 1].LastName}");
                                break;
                            }

                            Console.WriteLine("Invalid selection. Please enter a valid number.");
                        }
                    }
                }

                var updatedProject = new ProjectDto
                {
                    Id = selectedProject.Id, 
                    ProjectNumber = selectedProject.ProjectNumber,
                    Name = string.IsNullOrWhiteSpace(newName) ? selectedProject.Name : newName,
                    Description = string.IsNullOrWhiteSpace(newDescription) ? selectedProject.Description : newDescription,
                    StartDate = string.IsNullOrWhiteSpace(newStartDate) ? selectedProject.StartDate : DateTime.Parse(newStartDate),
                    EndDate = string.IsNullOrWhiteSpace(newEndDate) ? selectedProject.EndDate : DateTime.Parse(newEndDate),
                    StatusId = newStatusId,
                    CustomerId = newCustomerId,
                    ServiceId = newServiceId,
                    Duration = string.IsNullOrWhiteSpace(newServiceDuration) ? selectedProject.Duration : int.Parse(newServiceDuration),
                    EmployeeId = selectedEmployeeId
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
                await _projectService.UpdateProjectAsync(updatedProject);
                Console.WriteLine("\nProject updated successfully!");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
            }
        }
    }

    private async Task DeleteProjectAsync()
    {
        Console.Clear();
        Console.WriteLine("\tPROJECT-MANAGER");
        Console.WriteLine("\tDelete Project");

        Console.WriteLine("This Is The Project List:");

        var projectsResult = await _projectService.GetAllProjectAsync();

        if (projectsResult is Result<IEnumerable<ProjectDto>> projectResult && projectResult.Success)
        {
            var projects = projectResult.Data.ToList();

            if (!projects.Any())
            {
                Console.WriteLine("No projects found.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            int index = 1;
            foreach (var project in projects)
            {
                Console.WriteLine($"{index}. Name: {project.Name}");
                index++;
            }

            Console.WriteLine("----Which Project Would You Like To Delete?");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= projects.Count())
            {
                var selectedProject = projects.ElementAt(choice - 1);

                Console.WriteLine($"Are you sure you want to delete the project: {selectedProject.Name}? (y/n)");
                var confirmation = Console.ReadLine();

                if (confirmation?.ToLower() == "y")
                {
                    Console.WriteLine($"Deleting project: {selectedProject.Name}");

                    try
                    {
                        await _projectService.DeleteProjectAsync(new ProjectDto { Id = selectedProject.Id });
                        Console.WriteLine("Project deleted successfully.");
                        Console.WriteLine("Project not found.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete the Project. Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Project deletion cancelled.");
                }
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }
        else
        {
            Console.WriteLine("Failed to load projects or no projects available.");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }

}

