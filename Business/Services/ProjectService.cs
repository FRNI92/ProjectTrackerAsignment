﻿using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;

using Data_Infrastructure.Interfaces;
using Data_Infrastructure.Repositories;
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

        await _projectRepository.BeginTransactionAsync();
        try
        {
            var selectedService = await _projectRepository.GetServiceByIdAsync(projectDto.ServiceId);
            if (selectedService == null)
            {
                await _projectRepository.RollBackTransactionAsync();
                return Result.NotFound("Could not find a service with that service ID");
            }
            // Beräkna totalpris baserat på tjänstens pris och användarens valda duration
            var totalPrice = selectedService.Price * projectDto.Duration;

            var entity = ProjectFactory.ToEntity(projectDto);
            entity.TotalPrice = totalPrice; // Spara totalpriset i entiteten
            entity.Duration = projectDto.Duration; // Spara duration

            var result = await _projectRepository.AddAsync(entity); // Spara projektet
            if (!result)
            {
                await _projectRepository.RollBackTransactionAsync();
                return Result.Error("Unable To Create Project");
            }

            var saveResult = await _projectRepository.SaveAsync();
            if (saveResult > 0)
            {
                await _projectRepository.CommitTransactionAsync();
                var createdProjectDto = ProjectFactory.ToDto(entity);
                return Result<ProjectDto>.OK(createdProjectDto);
            }

            await _projectRepository.RollBackTransactionAsync();
            return Result.Error("Failed to create project, no changes were saved.");
        }
        catch (Exception ex)
        {
            await _projectRepository.RollBackTransactionAsync();
            Debug.WriteLine(ex.Message);
            return Result.Error("Something went wrong when creating project");
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

    public async Task<IResult> UpdateProjectAsync(ProjectDto projectDto)
    {

        await _projectRepository.BeginTransactionAsync();
        try
        {
            //Hämta projektet
            var fetcheduneditedProject = await _projectRepository.GetAsync(p => p.Id == projectDto.Id);
            if (fetcheduneditedProject == null)
            {
                await _projectRepository.RollBackTransactionAsync();
                return Result.Error("An error occurred while loading the project.");
            }

            //Hämta den nya tjänsten om ServiceId ändrats
            if (projectDto.ServiceId != fetcheduneditedProject.ServiceId)
            {
                var selectedService = await _projectRepository.GetServiceByIdAsync(projectDto.ServiceId);
                if (selectedService == null)
                {
                    await _projectRepository.RollBackTransactionAsync();
                    return Result.NotFound("cant find service with that ID");
                }

                //Uppdatera tjänsten och priset
                fetcheduneditedProject.ServiceId = projectDto.ServiceId;
                fetcheduneditedProject.TotalPrice = selectedService.Price * fetcheduneditedProject.Duration;
            }

            //Uppdatera durationen och räkna om priset om den ändrats
            if (projectDto.Duration != 0 && projectDto.Duration != fetcheduneditedProject.Duration)
            {
                fetcheduneditedProject.Duration = projectDto.Duration;
                var service = await _projectRepository.GetServiceByIdAsync(fetcheduneditedProject.ServiceId);
                if (service != null)
                {
                    fetcheduneditedProject.TotalPrice = service.Price * fetcheduneditedProject.Duration;
                }
            }

            //Uppdatera specifika fält för projektet
            fetcheduneditedProject.Name = string.IsNullOrWhiteSpace(projectDto.Name) ? fetcheduneditedProject.Name : projectDto.Name;
            fetcheduneditedProject.Description = string.IsNullOrWhiteSpace(projectDto.Description) ? fetcheduneditedProject.Description : projectDto.Description;
            fetcheduneditedProject.StartDate = projectDto.StartDate != DateTime.MinValue ? projectDto.StartDate : fetcheduneditedProject.StartDate;
            fetcheduneditedProject.EndDate = projectDto.EndDate.HasValue ? projectDto.EndDate : fetcheduneditedProject.EndDate;
            fetcheduneditedProject.StatusId = projectDto.StatusId != 0 ? projectDto.StatusId : fetcheduneditedProject.StatusId;
            fetcheduneditedProject.CustomerId = projectDto.CustomerId != 0 ? projectDto.CustomerId : fetcheduneditedProject.CustomerId;
            fetcheduneditedProject.ServiceId = projectDto.ServiceId != 0 ? projectDto.ServiceId : fetcheduneditedProject.ServiceId;
            fetcheduneditedProject.Duration = projectDto.Duration != 0 ? projectDto.Duration : fetcheduneditedProject.Duration;
            fetcheduneditedProject.EmployeeId = projectDto.EmployeeId != 0 ? projectDto.EmployeeId : fetcheduneditedProject.EmployeeId;
            // Skicka med både lambda-uttryck och den uppdaterade entiteten
            var updatedProject = await _projectRepository.TransactionUpdateAsync(p => p.Id == projectDto.Id, fetcheduneditedProject);

            if (updatedProject == null)
            {
                await _projectRepository.RollBackTransactionAsync();
                return Result.Error("Unable to update project.");
            }

            var saveResult = await _projectRepository.SaveAsync();
            if (saveResult > 0)
            {
                await _projectRepository.CommitTransactionAsync();
                var updatedProjectDto = ProjectFactory.ToDto(updatedProject);
                return Result<ProjectDto>.OK(updatedProjectDto);
            }

            await _projectRepository.RollBackTransactionAsync();
            return Result.Error("Update failed, no changes were savd.");
        }
        catch (Exception ex)
        {
            await _projectRepository.RollBackTransactionAsync();
            Debug.WriteLine($"Problem when updating project {ex.Message} {ex.StackTrace}");
            return Result.Error("Problem when updating project");
        }
    }
  
    public async Task<IResult> DeleteProjectAsync(ProjectDto projectDto) 
    {
        await _projectRepository.BeginTransactionAsync();
        try
        {
            var project = await _projectRepository.GetAsync(p => p.Id == projectDto.Id);
            if (project == null)
            {
                await _projectRepository.RollBackTransactionAsync();
                return Result.Error("Project could not be found");
            }

            var isRemoved = await _projectRepository.RemoveAsync(p => p.Id == projectDto.Id);
            if (!isRemoved)
            {
                await _projectRepository.RollBackTransactionAsync();
                return Result.Error("Failed to remove project.");
            }
            var isRemovedAndSaved = await _projectRepository.SaveAsync();
            if (isRemovedAndSaved > 0)
            {
                await _projectRepository.CommitTransactionAsync();
                return Result.OK();
            }

            await _projectRepository.RollBackTransactionAsync();
            return Result.Error("Deletion failed, no changes were saved.");
        }
        catch (Exception ex)
        {
            await _projectRepository.RollBackTransactionAsync();
            Debug.WriteLine($"Could not Delete project{ex.Message} {ex.StackTrace}");
            return Result.Error("Could not delete project");
        }
    }
}

