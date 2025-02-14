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

        // Hämta resultatet från din service
        var result = await _projectService.GetAllProjectAsync();

        // Kontrollera om resultatet är framgångsrikt och hämta datan
        if (result is Result<IEnumerable<ProjectDto>> projectResult)//man måste kolla att det är rätt typ för att komma åt .Data. är result av rätt typ så döpt den om till projectResult
        {
            // Om resultatet är av typen Result<IEnumerable<ProjectDto>>, kommer det att konverteras till projectResult
            var projects = projectResult.Data;
            Console.WriteLine($"DEBUG: Antal projekt i listan: {projects.Count()}");

            // Iterera genom projekten och skriv ut information
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
        // Tilldela det valda StatusId till det nya projektet



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
                    Console.WriteLine($"{i + 1}. {services[i].Name} {services[i].Description}");//int i+1 för att visa en siffra. sedan används int som index för att hitta rätt i listan
                }
                int selectedServiceIndex;//inget värde. ska användas senare
                while (true)
                {
                    Console.Write("Enter the number of the Service: ");
                    var input = Console.ReadLine();// Nu får användaren välja i "listan"


                    //is number och alla andra saker är true så flytta input till selectedServiceIndex
                    if (int.TryParse(input, out selectedServiceIndex) && selectedServiceIndex > 0 && selectedServiceIndex <= services.Count)
                    {
                        newProject.ServiceId = services[selectedServiceIndex - 1].Id;//för att kompensera för index -1
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
                    break; // Går ur loopen om inmatningen är giltig
                }

                Console.WriteLine("Invalid input. Please enter a valid number.");
            }

            newProject.Duration = duration;


            var createdProjectResult = await _projectService.CreateProjectAsync(newProject);

            if (createdProjectResult is Result<ProjectDto> createdProjectData)
            {
                // Om resultatet är framgångsrikt, hämta projektets namn
                var createdProject = createdProjectData.Data;  // Använd createdProjectData.Data
                Console.WriteLine($"Project created! Project name: {createdProject.Name} Total Cost: {createdProject.TotalPrice} :-");
            }
            else
            {
                // Om det inte är framgångsrikt, visa felmeddelande
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

        // Hämta alla kunder
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


            // Visa alla kunder
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

            // Låt användaren välja en kund
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= projectsData.Count())
            {
                var selectedProject = projectsData.ElementAt(choice - 1); //hämta vald kund

                Console.Write($"Enter new name (leave blank to keep current:({selectedProject.Name}): ");
                var newName = Console.ReadLine();

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
                // Tilldela det valda StatusId till det nya projektet


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

                var newServiceId = selectedProject.ServiceId; // Behåll nuvarande värde
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
                        Console.WriteLine($"{i + 1}. {services[i].Name} {services[i].Description}");//int i+1 för att visa en siffra. sedan används int som index för att hitta rätt i listan
                    }
                    int selectedServiceIndex;//inget värde. ska användas senare
                    while (true)
                    {
                        Console.Write("Enter the number of the Service: ");
                        var input = Console.ReadLine();// Nu får användaren välja i "listan"


                        //is number och alla andra saker är true så flytta input till selectedServiceIndex
                        if (int.TryParse(input, out selectedServiceIndex) && selectedServiceIndex > 0 && selectedServiceIndex <= services.Count)
                        {
                            newServiceId = services[selectedServiceIndex - 1].Id;//för att kompensera för index -1
                            break;
                        }
                        Console.WriteLine("Invalid selection. Please enter a valid number.");
                    }
                }

                Console.Write($"Enter new Service-Duration:(leave blank to keep current:({selectedProject.Duration}): ");
                var newServiceDuration = Console.ReadLine();


                var employeesResult = await _employeeService.GetEmployeeAsync();//hämmta listan
                int selectedEmployeeId = selectedProject.EmployeeId;//behåll det man har från början
                //kollar om employeesResult är av rätt typ och får då ut en employeeResult som man också kolla om den är success
                if (employeesResult is Result<IEnumerable<EmployeeDto>> employeeResult && employeeResult.Success)
                {
                    var employees = employeeResult.Data.ToList();//flyttar över datan till employees

                    if (employees.Any())//gör detta om det finns ngt
                    {
                        Console.WriteLine("\nSelect New Project Manager (Leave blank to keep current):");
                        for (int i = 0; i < employees.Count; i++)//kör +1 sålänge det är färre än count
                        {
                            //skriver ut i+1 och employee med det index first name och employee index lastame
                            Console.WriteLine($"{i + 1}. {employees[i].FirstName} {employees[i].LastName}");
                        }

                        while (true)//kör tills det breakar
                        {
                            Console.Write("\nEnter the number of the new Project Manager (or press Enter to keep current): ");
                            var input = Console.ReadLine();//tar input

                            if (string.IsNullOrWhiteSpace(input))//kollar att det är rätt input
                            {
                                Console.WriteLine($"Keeping current Project Manager: {selectedProject.EmployeeId}");
                                break;//tar den gamla om det är fel input
                            }
                            //kollar om input är av typen int och är det sant så blir den employeeChoice om den är mer än 0 och färre än employees.count
                            if (int.TryParse(input, out int employeeChoice) && employeeChoice > 0 && employeeChoice <= employees.Count)
                            {
                                selectedEmployeeId = employees[employeeChoice - 1].Id;//vvalet man gjort minus 1 pågrund av index
                                Console.WriteLine($"Selected new Project Manager: {employees[employeeChoice - 1].FirstName} {employees[employeeChoice - 1].LastName}");
                                break;//samma sak när den skriver ut. valet minus 1 på index
                            }

                            Console.WriteLine("Invalid selection. Please enter a valid number.");
                        }
                    }
                }



                // Skapa DTO med det nya värdet
                var updatedProject = new ProjectDto
                {
                    Id = selectedProject.Id, // Skicka ID:t istället för att söka med namn
                    ProjectNumber = selectedProject.ProjectNumber,
                    Name = string.IsNullOrWhiteSpace(newName) ? selectedProject.Name : newName,//tom eller null behåll gamla : annar newName
                    Description = string.IsNullOrWhiteSpace(newDescription) ? selectedProject.Description : newDescription,
                    StartDate = string.IsNullOrWhiteSpace(newStartDate) ? selectedProject.StartDate : DateTime.Parse(newStartDate),//behöver konverteras
                    EndDate = string.IsNullOrWhiteSpace(newEndDate) ? selectedProject.EndDate : DateTime.Parse(newEndDate),
                    StatusId = newStatusId,
                    CustomerId = newCustomerId,
                    ServiceId = newServiceId,
                    Duration = string.IsNullOrWhiteSpace(newServiceDuration) ? selectedProject.Duration : int.Parse(newServiceDuration),
                    EmployeeId = selectedEmployeeId
                };

                // Uppdatera kunden
                await _projectService.UpdateProjectAsync(updatedProject);
                Console.WriteLine("\nProject updated successfully!");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
            }
        }
    }


    //Användaren väljer en kund via index i en lista.
    //En DTO skapas och skickas till service-lagret med ID:t för den valda kunden.
    //Service-lagret hämtar kunden via repositoryt och kontrollerar att den existerar.
    //Om kunden finns, skickas entiteten vidare till repositoryt.
    //Repositoryt markerar entiteten som ska tas bort och sparar ändringen i databasen.
    private async Task DeleteProjectAsync()
    {
        Console.Clear();
        Console.WriteLine("\tPROJECT-MANAGER");
        Console.WriteLine("\tDelete Project");

        Console.WriteLine("This Is The Project List:");

        // Hämta alla projekt
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

            // Visa alla kunder
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

                // Be om bekräftelse innan radering
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

