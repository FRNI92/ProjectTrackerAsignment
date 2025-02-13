using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Diagnostics;

namespace Business.Services;

public class StatusService(IStatusRepository statusRepository)
{
    private readonly IStatusRepository _statusRepository = statusRepository;

    //CREATE
    public async Task<IResult> CreateStatusAsync(StatusDto statusDto)
    {
        try
        {
            if (statusDto == null)
                return Result.Error("status dto was not filled in correctly");

            var isExisting = await _statusRepository.DoesEntityExistAsync(s => s.Name == statusDto.Name);
            if (isExisting)
            {
                return Result.AlreadyExists("A status with that name already exists.");
            }

            var newStatusEntity = StatusFactory.ToEntity(statusDto);
            var createdStatusEntity = await _statusRepository.CreateAsync(newStatusEntity);
            if (createdStatusEntity)
                return Result.OK();
            return Result.Error("Could not create status correctly");
            
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred when creating status: {ex.Message}{ex.StackTrace}");
            return Result.Error("There was a problem with server");
        }
    }

    public async Task<IResult> ReadStatusAsync()
    {
        try
        {
            var statusEntity = await _statusRepository.GetAllAsync();
            if (!statusEntity.Any())
            {
                return Result.NotFound("No statuses found");
            }
            else
            {
                var statusDto = statusEntity.Select(StatusFactory.ToDto).ToList();
                return Result<IEnumerable<StatusDto>>.OK(statusDto);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"There was a problem with getting statuses:{ex.Message}{ex.StackTrace}");
            return Result.Error("There was an error when loading statuses");

        }
    }

    public async Task <IResult> UpdateStatusAsync(StatusDto statusDto) 
    {
        try
        {
            if (statusDto == null)
            {
                return Result.BadRequest("statusdto cant be null");
            }

            // Hämta den befintliga entiteten från databasen
            var existingEntity = await _statusRepository.GetAsync(s => s.Id == statusDto.Id);
            if (existingEntity == null)
            {
                Console.WriteLine("Status not found.");
                return Result.NotFound("Could not find the status");
            }
            else
            {
            StatusFactory.UpdatedEntity(existingEntity, statusDto);

            
             var updatedStatus = await _statusRepository.UpdateAsync(s => s.Id == statusDto.Id, existingEntity);
                if (updatedStatus != null)
                {
                    return Result<StatusDto>.OK(StatusFactory.ToDto(updatedStatus));
                }
                return Result.NotFound("Could not find status to update");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while updating the status: {ex.Message}{ex.StackTrace}");
            return Result.Error("There was an error when updating status");
        }
    }


    public async Task<IResult> DeleteStatusAsync(int id)
    {
        try
        {
            // Kontrollera om statusen finns
            var exists = await _statusRepository.DoesEntityExistAsync(s => s.Id == id);
            if (!exists)
            {
                return Result.NotFound("Could not find the status");
            }

            var wasDeleted = await _statusRepository.DeleteAsync(s => s.Id == id);
            if (wasDeleted)
                return Result.OK();

            return Result.BadRequest("was not able to delete status");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred when deleting status: {ex.Message}{ex.StackTrace}");
            return Result.Error("something went wrong when deleting the status");
        }
    }    
}
