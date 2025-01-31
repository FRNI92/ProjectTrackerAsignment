
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

    public async Task<IEnumerable<ProjectDto>> GetAllProjectAsync()
    {
        var allProjects = await _projectRepository.GetAllAsync();

        return allProjects.Select(ProjectFactory.ToDto).ToList();

    }

    // Den här metoden använder redan den gemensamma UpdateAsync från BaseService
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

        // Skicka med både lambda-uttryck och den uppdaterade entiteten
        var updatedProject = await UpdateAsync(p => p.Id == projectDto.Id, fetcheduneditedProject);

        // Returnera den uppdaterade projektet som DTO
        return ProjectFactory.ToDto(updatedProject);
    }

    // Den här metoden använder redan den gemensamma CreateAsync från BaseService
    public async Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto)
    {
        var entity = ProjectFactory.ToEntity(projectDto); // Mappa DTO till Entity
        var createdEntity = await CreateAsync(entity); // Skapa entitet
        return ProjectFactory.ToDto(createdEntity); // Mappa tillbaka till DTO  
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

