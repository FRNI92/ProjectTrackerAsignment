using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;

namespace Presentation.MenuDialogs;

public class ProjectMenuDialogs
{
    private readonly IProjectService _projectService;
    private readonly EmployeeService _employeeService;

    public ProjectMenuDialogs(IProjectService projectService, EmployeeService employeeService)
    {
        _projectService = projectService;
        _employeeService = employeeService;

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
            // Om det inte finns några projekt, eller om det är något fel
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

        Console.Write("Enter Status ID (1 = Not started, 2 = In Progress, 3 = Completed): ");
        newProject.StatusId = int.Parse(Console.ReadLine()!);

        //Console.WriteLine("Select Project Manager:");
        //Console.WriteLine("1 = Anna Andersson, 2 = Johan Johansson, 3 = Maria Karlsson");
        //Console.Write("Enter Employee ID: ");
        //newProject.EmployeeId = int.Parse(Console.ReadLine()!); var bättre att foreacha än ha "hårdkoda" valen i menyn
        var employees = await _employeeService.GetAllEmployeesAsync();

        Console.WriteLine("Select Project Manager:");
        foreach (var employee in employees)
        {
            Console.WriteLine($"{employee.Id} = {employee.FirstName} {employee.LastName}");
        }

        Console.Write("Enter Employee ID: ");
        newProject.EmployeeId = int.Parse(Console.ReadLine()!);

        Console.WriteLine("Select Customer (Company who ordered the project):");
        Console.WriteLine("1 = Company A, 2 = Company B, 3 = Company C");
        Console.Write("Enter Customer ID: ");
        newProject.CustomerId = int.Parse(Console.ReadLine()!);

        Console.Write("Enter Service ID: (1 = IT-Support:, 2 = Consulting):");
        newProject.ServiceId = int.Parse(Console.ReadLine()!);

        Console.Write("Enter Duration (in hours): ");
        newProject.Duration = decimal.Parse(Console.ReadLine()!);



        var createdProject = await _projectService.CreateProjectAsync(newProject);
        Console.WriteLine($"Project created:");
        Console.ReadKey();
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

                Console.Write($"Enter new Status-ID (1 = Not started, 2 = In Progress, 3 = Completed): ) (leave blank to keep current:({selectedProject.StatusId}): ");
                var newStatusId = Console.ReadLine();

                Console.Write($"Enter new Customer-ID (1 = Company A:, 2 = Companny B): (leave blank to keep current:({selectedProject.CustomerId}): ");
                var newCustomerId = Console.ReadLine();

                Console.Write($"Enter new Service ID: (1 = IT-Support:, 2 = Consulting): (leave blank to keep current:({selectedProject.ServiceId}): ");
                var newServiceId = Console.ReadLine();

                Console.Write($"Enter new Service-Duration:(leave blank to keep current:({selectedProject.Duration}): ");
                var newServiceDuration = Console.ReadLine();

                Console.Write($"Enter New Project Manager (Employee ID): (1 = Anna : 2 = Johan)(leave blank to keep current:({selectedProject.EmployeeId}): ");
                var newEmployeeId = Console.ReadLine();

                // Skapa DTO med det nya värdet
                var updatedProject = new ProjectDto
                {
                    Id = selectedProject.Id, // Skicka ID:t istället för att söka med namn
                    ProjectNumber = selectedProject.ProjectNumber,
                    Name = string.IsNullOrWhiteSpace(newName) ? selectedProject.Name : newName,//tom eller null behåll gamla : annar newName
                    Description = string.IsNullOrWhiteSpace(newDescription) ? selectedProject.Description : newDescription,
                    StartDate = string.IsNullOrWhiteSpace(newStartDate) ? selectedProject.StartDate : DateTime.Parse(newStartDate),//behöver konverteras
                    EndDate = string.IsNullOrWhiteSpace(newEndDate) ? selectedProject.EndDate : DateTime.Parse(newEndDate),
                    StatusId = string.IsNullOrWhiteSpace(newStatusId) ? selectedProject.StatusId : int.Parse(newStatusId),
                    CustomerId = string.IsNullOrWhiteSpace(newCustomerId) ? selectedProject.CustomerId : int.Parse(newCustomerId),
                    ServiceId = string.IsNullOrWhiteSpace(newServiceId) ? selectedProject.ServiceId : int.Parse(newServiceId),
                    Duration = string.IsNullOrWhiteSpace(newServiceDuration) ? selectedProject.Duration : int.Parse(newServiceDuration),
                    EmployeeId = string.IsNullOrWhiteSpace(newEmployeeId) ? selectedProject.EmployeeId : int.Parse(newEmployeeId),
                };

                // Uppdatera kunden
                await _projectService.UpdateProjectAsync(updatedProject);
                Console.WriteLine("\nCustomer updated successfully!");
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
    public async Task DeleteProjectAsync()
    {
        Console.Clear();
        Console.WriteLine("\tCUSTOMER-MANAGER");
        Console.WriteLine("\tDelete Customer");

        Console.WriteLine("This Is The Customer List:");

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

