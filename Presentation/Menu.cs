﻿using Business.Dtos;
using Business.Interfaces;
using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Presentation;

public class Menu
{
    private readonly IProjectService _projectService;

    public Menu(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task ShowMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Show Preview Of All Projects");
            Console.WriteLine("2. Create a new Project");
            Console.WriteLine("3. Show Detailed View And Update a Project");
            Console.WriteLine("4. Delete a Project");
            Console.WriteLine("0. Exit");

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
                    return; // Exit program
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    private async Task ShowProjectsPreviewAsync()
    {
        Console.Clear();
        Console.WriteLine("DEBUG: ShowProjectsPreviewAsync() körs");

        var Projects = await _projectService.GetAllProjectAsync();
        Console.WriteLine($"DEBUG: Antal projekt i listan: {Projects.Count()}");


        foreach (var project in Projects)
        {
            Console.WriteLine($"Project ID:{project.Id}");
            Console.WriteLine($"Project Number:{project.ProjectNumber}");
            Console.WriteLine($"Name: {project.Name}");
            Console.WriteLine($"Time Period: {project.StartDate} - {project.EndDate}");
            //Console.WriteLine($"Status ID: {project.StatusId}"); behövs inte
            Console.WriteLine($"Current Status: {project.StatusName}");
            Console.WriteLine(); // För att ge lite mellanrum mellan projekten
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

        Console.Write("Enter Customer ID:  (1 = Company A:, 2 = Companny B): ");
        newProject.CustomerId = int.Parse(Console.ReadLine()!);

        Console.Write("Enter Service ID: (1 = IT-Support:, 2 = Consulting):");
        newProject.ServiceId = int.Parse(Console.ReadLine()!);

        Console.Write("Enter Project Manager (Employee ID): (1 = Anna : 2 = Johan)");
        newProject.EmployeeId = int.Parse(Console.ReadLine()!);


        var createdProject = await _projectService.CreateProjectAsync(newProject);
        Console.WriteLine($"Project created: {createdProject.Name}");
        Console.ReadKey();
    }

    private async Task UpdateProjectAsync()
    {
        Console.Clear();
        Console.WriteLine("This Is The Projects List:");

        // Hämta alla kunder
        var projects = await _projectService.GetAllProjectAsync();

        if (!projects.Any())
        {
            Console.WriteLine("No Projects found.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        // Visa alla kunder
        int index = 1;
        foreach (var project in projects)
        {
            Console.WriteLine($"{index}.ProjectNumber: {project.ProjectNumber}\t Name: {project.Name}");
            Console.WriteLine($"Current Status: {project.StatusName}");
            Console.WriteLine($"Customer Name: {project.CustomerName}");
            Console.WriteLine($"Employee Name: {project.EmployeeName}");
            Console.WriteLine($"The Service Purchased: {project.ServiceName}");
         
            Console.WriteLine("------------------------------------");
            index++;
        }

        Console.WriteLine("----Which Project Would You Like To Update? Enter the number:");

        // Låt användaren välja en kund
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= projects.Count())
        {
            var selectedProject = projects.ElementAt(choice - 1); //hämta vald kund

            Console.Write("Enter new name (leave blank to keep current): ");
            var newName = Console.ReadLine();

            Console.Write("Enter new Description (leave blank to keep current): ");
            var newDescription = Console.ReadLine();

            Console.Write("Enter new StartDate (leave blank to keep current): ");
            var newStartDate = Console.ReadLine();

            Console.Write("Enter new EndDate (leave blank to keep current): ");
            var newEndDate = Console.ReadLine();

            Console.Write("Enter new Status-ID (1 = Not started, 2 = In Progress, 3 = Completed): ) (leave blank to keep current): ");
            var newStatusId = Console.ReadLine();

            Console.Write("Enter new Customer-ID (1 = Company A:, 2 = Companny B): (leave blank to keep current): ");
            var newCustomerId = Console.ReadLine();

            Console.Write("Enter new Service ID: (1 = IT-Support:, 2 = Consulting): (leave blank to keep current): ");
            var newServiceId = Console.ReadLine();

            Console.Write("Enter New Project Manager (Employee ID): (1 = Anna : 2 = Johan)(leave blank to keep current): ");
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
                EmployeeId = string.IsNullOrWhiteSpace(newEmployeeId) ? selectedProject.EmployeeId : int.Parse(newEmployeeId),
            };

            // Uppdatera kunden
            await _projectService.UpdateProjectAsync(updatedProject);
            Console.WriteLine("\nCustomer updated successfully!");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
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

        // Hämta alla kunder
        var projects = await _projectService.GetAllProjectAsync();

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

            Console.WriteLine($"Deleting customer: {selectedProject.Name}");

            try
            {
                // Anropa Delete via Projectservce
                await _projectService.DeleteProjectAsync(new ProjectDto { Id = selectedProject.Id });//skapar en dto och sätter id till samma som vi valt via index
                Console.WriteLine("Project deleted successfully.");
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("Project not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete the Project. Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}
