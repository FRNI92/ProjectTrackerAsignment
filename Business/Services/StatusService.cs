using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Diagnostics;

namespace Business.Services;

public class StatusService(IStatusRepository statusRepository) : IStatusService
{
    private readonly IStatusRepository _statusRepository = statusRepository;


    public async Task<IResult> CreateStatusAsync(StatusDto statusDto)
    {
        if (statusDto == null)
            return Result.BadRequest("Status DTO was not filled in correctly");

        var isExisting = await _statusRepository.DoesEntityExistAsync(s => s.Name == statusDto.Name);
        if (isExisting)
            return Result.AlreadyExists("A status with that name already exists.");

        await _statusRepository.BeginTransactionAsync();
        try
        {
            var newStatusEntity = StatusFactory.ToEntity(statusDto);
            await _statusRepository.AddAsync(newStatusEntity); 

            var saveResult = await _statusRepository.SaveAsync(); 
            if (saveResult > 0)
            {
                await _statusRepository.CommitTransactionAsync();
                return Result.OK();
            }

            await _statusRepository.RollBackTransactionAsync();
            return Result.Error("Could not create status correctly");
        }
        catch (Exception ex)
        {
            await _statusRepository.RollBackTransactionAsync();
            Debug.WriteLine($"An error occurred when creating status: {ex.Message}{ex.StackTrace}");
            return Result.Error("There was a problem with the server");
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

    public async Task<IResult> UpdateStatusAsync(StatusDto statusDto)
    {
        if (statusDto == null)
        {
            return Result.BadRequest("statusdto cant be null");
        }

        await _statusRepository.BeginTransactionAsync();
        try
        {
         
            var existingEntity = await _statusRepository.GetAsync(s => s.Id == statusDto.Id);
            if (existingEntity == null)
            {
                await _statusRepository.RollBackTransactionAsync();
                return Result.NotFound("Could not find the status");
            }
            else
            {
                var updateEntity = StatusFactory.ToEntity(statusDto);
                var updatedStatus = await _statusRepository.TransactionUpdateAsync(s => s.Id == statusDto.Id, updateEntity);

                if (updatedStatus != null)
                {
                    var saveResult = await _statusRepository.SaveAsync();
                    if (saveResult > 0)
                    {
                        await _statusRepository.CommitTransactionAsync();
                        return Result<StatusDto>.OK(StatusFactory.ToDto(updatedStatus));
                    }
                }
                await _statusRepository.RollBackTransactionAsync();
                return Result.Error("Could not update status");
            }
        }
        catch (Exception ex)
        {
            await _statusRepository.RollBackTransactionAsync();
            Debug.WriteLine($"An error occurred while updating the status: {ex.Message}{ex.StackTrace}");
            return Result.Error("There was an error when updating status");
        }
    }


    public async Task<IResult> DeleteStatusAsync(int id)
    {
        await _statusRepository.BeginTransactionAsync();
        try
        {
            var exists = await _statusRepository.DoesEntityExistAsync(s => s.Id == id);
            if (!exists)
            {
                await _statusRepository.RollBackTransactionAsync();
                return Result.NotFound("Could not find the status");
            }

            var wasDeleted = await _statusRepository.RemoveAsync(s => s.Id == id);
            if (wasDeleted)
            {
                var saveResult = await _statusRepository.SaveAsync();
                if (saveResult > 0)
                {
                    await _statusRepository.CommitTransactionAsync();
                    return Result.OK();
                }
            }

            await _statusRepository.RollBackTransactionAsync();
            return Result.BadRequest("Was not able to delete status");
        }
        catch (Exception ex)
        {
            await _statusRepository.RollBackTransactionAsync();
            Debug.WriteLine($"An error occurred when deleting status: {ex.Message}{ex.StackTrace}");
            return Result.Error("something went wrong when deleting the status");
        }
    }
}
