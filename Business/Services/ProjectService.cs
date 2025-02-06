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

public class ProjectService : BaseService<ProjectEntity>, IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository) : base(projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto)
    {
        var selectedService = await _projectRepository.GetServiceByIdAsync(projectDto.ServiceId);
        if (selectedService == null)
        {
            throw new KeyNotFoundException("Service not found");
        }

        // Beräkna totalpris baserat på tjänstens pris och användarens valda duration
        var totalPrice = selectedService.Price * projectDto.ServiceDuration;

        // Mappa DTO till Entity
        var entity = ProjectFactory.ToEntity(projectDto);
        entity.TotalPrice = totalPrice; // Spara totalpriset i entiteten
        entity.Duration = projectDto.ServiceDuration; // Spara duration

        var createdEntity = await _projectRepository.CreateAsync(entity); // Spara projektet
        return ProjectFactory.ToDto(createdEntity);
    }

    public async Task<IEnumerable<ProjectDto>> GetAllProjectAsync()
    {
        var allProjects = await _projectRepository.GetAllAsync();

        return allProjects.Select(ProjectFactory.ToDto).ToList();

    }

    public async Task<ProjectDto> UpdateProjectAsync(ProjectDto projectDto)
    {
        // Hämta projektet
        var fetcheduneditedProject = await GetAsync(p => p.Id == projectDto.Id);
        if (fetcheduneditedProject == null)
        {
            throw new KeyNotFoundException("Project could not be found");
        }

        // Uppdatera specifika fält för projektet
        fetcheduneditedProject.Name = string.IsNullOrWhiteSpace(projectDto.Name) ? fetcheduneditedProject.Name : projectDto.Name;
        fetcheduneditedProject.Description = string.IsNullOrWhiteSpace(projectDto.Description) ? fetcheduneditedProject.Description: projectDto.Description;
        fetcheduneditedProject.StartDate = projectDto.StartDate != DateTime.MinValue ? projectDto.StartDate : fetcheduneditedProject.StartDate;
        fetcheduneditedProject.EndDate = projectDto.EndDate.HasValue ? projectDto.EndDate : fetcheduneditedProject.EndDate;

        fetcheduneditedProject.StatusId = projectDto.StatusId != 0 ? projectDto.StatusId : fetcheduneditedProject.StatusId;
        fetcheduneditedProject.CustomerId = projectDto.CustomerId != 0 ? projectDto.CustomerId : fetcheduneditedProject.CustomerId;
        fetcheduneditedProject.ServiceId = projectDto.ServiceId != 0 ? projectDto.ServiceId : fetcheduneditedProject.ServiceId;
        fetcheduneditedProject.EmployeeId = projectDto.EmployeeId != 0 ? projectDto.EmployeeId : fetcheduneditedProject.EmployeeId;
        // Skicka med både lambda-uttryck och den uppdaterade entiteten
        var updatedProject = await UpdateAsync(p => p.Id == projectDto.Id, fetcheduneditedProject);

        // Returnera den uppdaterade projektet som DTO
        return ProjectFactory.ToDto(updatedProject);
    }
  
    public async Task DeleteProjectAsync(ProjectDto projectDto) // Använd samma GetAsync och DeleteAsync från BaseService här också
    {
        var project = await GetAsync(p => p.Id == projectDto.Id);
        if (project == null)
        {
            throw new KeyNotFoundException("Project could not be found");
        }

        await DeleteAsync(p => p.Id == projectDto.Id); // Använd DeleteAsync från BaseService
        Debug.WriteLine($"Project {projectDto.Id} deleted successfully.");
    }
}

