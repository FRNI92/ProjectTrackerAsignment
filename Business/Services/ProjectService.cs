using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Business.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IResult> CreateProjectAsync(ProjectDto projectDto)
    {
        if (projectDto == null)
            return Result.BadRequest("Invalid project Data transfer object");
        try
        {
            var selectedService = await _projectRepository.GetServiceByIdAsync(projectDto.ServiceId);
            if (selectedService == null)
            {
                return Result.NotFound("Could not find a service with that service ID");
            }
            // Beräkna totalpris baserat på tjänstens pris och användarens valda duration
            var totalPrice = selectedService.Price * projectDto.Duration;

            var entity = ProjectFactory.ToEntity(projectDto);
            entity.TotalPrice = totalPrice; // Spara totalpriset i entiteten
            entity.Duration = projectDto.Duration; // Spara duration

            var result = await _projectRepository.CreateAsync(entity); // Spara projektet
            return result ? Result.OK() : Result.Error("Unable To Create Project");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return Result.Error(ex.Message);
        }
    }

    public async Task<IResult> GetAllProjectAsync()
    {
        try
        {
            var allProjects = await _projectRepository.GetAllAsync();
            if (allProjects.Any())
            {
                var projectDtos = allProjects.Select(ProjectFactory.ToDto).ToList();
                return Result<IEnumerable<ProjectDto>>.OK(projectDtos);
            }

            else
            {
                return Result.NotFound("Could not find any project");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"There was an error when using GetAllAsync {ex.Message} {ex.StackTrace}");
            return Result.Error("There was an error when getting the projects");
        }
    }

    public async Task<ProjectDto> UpdateProjectAsync(ProjectDto projectDto)
    {
        // Hämta projektet
        var fetcheduneditedProject = await _projectRepository.GetAsync(p => p.Id == projectDto.Id);
        if (fetcheduneditedProject == null)
        {
            throw new KeyNotFoundException("Project could not be found");
        }

        // 🔹 Hämta den nya tjänsten om ServiceId ändrats
        if (projectDto.ServiceId != fetcheduneditedProject.ServiceId)
        {
            var selectedService = await _projectRepository.GetServiceByIdAsync(projectDto.ServiceId);
            if (selectedService == null)
            {
                throw new KeyNotFoundException("Service not found");
            }

            // 🔹 Uppdatera tjänsten och priset
            fetcheduneditedProject.ServiceId = projectDto.ServiceId;
            fetcheduneditedProject.TotalPrice = selectedService.Price * fetcheduneditedProject.Duration;
        }

        // 🔹 Uppdatera durationen och räkna om priset om den ändrats
        if (projectDto.Duration != 0 && projectDto.Duration != fetcheduneditedProject.Duration)
        {
            fetcheduneditedProject.Duration = projectDto.Duration;
            var service = await _projectRepository.GetServiceByIdAsync(fetcheduneditedProject.ServiceId);
            if (service != null)
            {
                fetcheduneditedProject.TotalPrice = service.Price * fetcheduneditedProject.Duration;
            }
        }

        // Uppdatera specifika fält för projektet
        fetcheduneditedProject.Name = string.IsNullOrWhiteSpace(projectDto.Name) ? fetcheduneditedProject.Name : projectDto.Name;
        fetcheduneditedProject.Description = string.IsNullOrWhiteSpace(projectDto.Description) ? fetcheduneditedProject.Description: projectDto.Description;
        fetcheduneditedProject.StartDate = projectDto.StartDate != DateTime.MinValue ? projectDto.StartDate : fetcheduneditedProject.StartDate;
        fetcheduneditedProject.EndDate = projectDto.EndDate.HasValue ? projectDto.EndDate : fetcheduneditedProject.EndDate;

        fetcheduneditedProject.StatusId = projectDto.StatusId != 0 ? projectDto.StatusId : fetcheduneditedProject.StatusId;
        fetcheduneditedProject.CustomerId = projectDto.CustomerId != 0 ? projectDto.CustomerId : fetcheduneditedProject.CustomerId;
        fetcheduneditedProject.ServiceId = projectDto.ServiceId != 0 ? projectDto.ServiceId : fetcheduneditedProject.ServiceId;
        fetcheduneditedProject.Duration = projectDto.Duration != 0 ? projectDto.Duration : fetcheduneditedProject.Duration;
        fetcheduneditedProject.EmployeeId = projectDto.EmployeeId != 0 ? projectDto.EmployeeId : fetcheduneditedProject.EmployeeId;
        // Skicka med både lambda-uttryck och den uppdaterade entiteten
        var updatedProject = await _projectRepository.UpdateAsync(p => p.Id == projectDto.Id, fetcheduneditedProject);

        // Returnera den uppdaterade projektet som DTO
        return ProjectFactory.ToDto(updatedProject);
    }
  
    public async Task DeleteProjectAsync(ProjectDto projectDto) 
    {
        var project = await _projectRepository.GetAsync(p => p.Id == projectDto.Id);
        if (project == null)
        {
            throw new KeyNotFoundException("Project could not be found");
        }

        await _projectRepository.DeleteAsync(p => p.Id == projectDto.Id); 
        Debug.WriteLine($"Project {projectDto.Id} deleted successfully.");
    }
}

