using Business.Dtos;
using Business.Factories;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Business.Services;

public class StatusService(IStatusRepository statusRepository)
{
    private readonly IStatusRepository _statusRepository = statusRepository;

    //CREATE
    public async Task<StatusDto> CreateStatusAsync(StatusDto statusDto)
    {
        try
        {
            if (statusDto == null)
                throw new ArgumentNullException(nameof(statusDto), "Status cannot be null.");

            var isExisting = await _statusRepository.DoesEntityExistAsync(s => s.Name == statusDto.Name);
            if (isExisting)
            {
                Console.WriteLine("A status with that name already exists.");
                throw new InvalidOperationException("A status with that name already exists.");
            }

            var newStatusEntity = StatusFactory.ToEntity(statusDto);
            var createdStatusEntity = await _statusRepository.CreateAsync(newStatusEntity);

            return StatusFactory.ToDto(createdStatusEntity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred when creating status: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }


    public async Task<IEnumerable<StatusDto>> ReadStatusAsync()
    {
        try
        {
            var statusEntity = await _statusRepository.GetAllAsync();
            if (!statusEntity.Any())
            {
                Console.WriteLine("No statuses found");
                return [];
            }
            else
            {
                var statusDto = statusEntity.Select(StatusFactory.ToDto);
                return statusDto;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was a problem with getting statuses:{ex.Message}");
            Console.WriteLine(ex.StackTrace)
            throw;
        }
    }

    public async Task <StatusDto> UpdateStatusAsync(StatusDto statusDto) 
    {
        try
        {
            if (statusDto == null)
            {
                Console.WriteLine("StatusDto cannot be null.");
                throw new ArgumentNullException(nameof(statusDto), "StatusDto cannot be null.");
            }

            // Hämta den befintliga entiteten från databasen
            var existingEntity = await _statusRepository.GetAsync(s => s.Id == statusDto.Id);
            if (existingEntity == null)
            {
                Console.WriteLine("Status not found.");
                throw new InvalidOperationException("Status not found.");
            }
            else
            {
            StatusFactory.UpdatedEntity(existingEntity, statusDto);

            await _statusRepository.UpdateAsync(s => s.Id == statusDto.Id, existingEntity);

            return StatusFactory.ToDto(existingEntity);

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while updating the status: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    public async Task<bool> DeleteStatusAsync(int id)
    {
        try
        {
            // Kontrollera om statusen finns
            var exists = await _statusRepository.DoesEntityExistAsync(s => s.Id == id);
            if (!exists)
            {
                Console.WriteLine("Status not found.");
                throw new InvalidOperationException("Status not found.");
            }

            // Om statusen finns, radera den
            return await _statusRepository.DeleteAsync(s => s.Id == id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred when deleting status: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

}
