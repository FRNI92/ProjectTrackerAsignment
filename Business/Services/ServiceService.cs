using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data_Infrastructure.Interfaces;
using Data_Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics;

namespace Business.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _serviceRepository;

    public ServiceService(IServiceRepository servicerepository)
    {
        _serviceRepository = servicerepository;
    }
    public async Task<IResult> CreateServiceAsync(ServiceDto serviceDto)
    {
        if (serviceDto == null)
            return Result.BadRequest("The service dto was not filled correctly");

        await _serviceRepository.BeginTransactionAsync();
        try
        {
            if (await _serviceRepository.DoesEntityExistAsync(s => s.Name == serviceDto.Name))
            {
                await _serviceRepository.RollBackTransactionAsync();
                return Result.AlreadyExists("Service with that name already exists");
            }

            var newServiceEntity = ServiceFactory.ToEntity(serviceDto);
            await _serviceRepository.AddAsync(newServiceEntity);

            if (await _serviceRepository.SaveAsync() > 0)
            {
                await _serviceRepository.CommitTransactionAsync();
                return Result.OK();
            }

            await _serviceRepository.RollBackTransactionAsync();
            return Result.Error("Could not create service correctly");
        }
        catch (Exception ex)
        {
            await _serviceRepository.RollBackTransactionAsync();
            Debug.WriteLine($"Error in CreateServiceAsync: {ex.Message} {ex.StackTrace}");
            return Result.BadRequest("Something went wrong when creating service");
        }
    }

    public async Task<IResult> ReadServiceAsync()
    {
        try
        {
            var serviceList = await _serviceRepository.GetAllAsync();
            if (!serviceList.Any())

                return Result.NotFound("could not find any services");


            var convertedList = serviceList.Select(ServiceFactory.ToDto).ToList();
            return Result<IEnumerable<ServiceDto>>.OK(convertedList);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occured when Reading Services: {ex.Message}{ex.StackTrace}");
            return Result.Error("There was an error when reading services");
        }
    }


    public async Task<IResult> ReadServiceByIdAsync(int serviceId)
    {
        try
        {
            var service = await _serviceRepository.GetAsync(s => s.Id == serviceId);
            if (service == null)
                return Result.NotFound("Service not found");

            var serviceDto = ServiceFactory.ToDto(service);
            return Result<ServiceDto>.OK(serviceDto);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred when reading service: {ex.Message}{ex.StackTrace}");
            return Result.Error("There was an error when reading the service");
        }
    }


    public async Task<IResult> UpdateServiceAsync(ServiceDto serviceDto)
    {
        if (serviceDto == null)
            return Result.BadRequest("serviceDto cant be empty");

        await _serviceRepository.BeginTransactionAsync();
        try
        {
            var existingEntity = await _serviceRepository.GetAsync(s => s.Id == serviceDto.Id);
            if (existingEntity == null)
            {
                await _serviceRepository.RollBackTransactionAsync();
                return Result.NotFound("Could not find the service to update");
            }

            // keep the old value if empty
            existingEntity.Name = string.IsNullOrWhiteSpace(serviceDto.Name) ? existingEntity.Name : serviceDto.Name;
            existingEntity.Description = string.IsNullOrWhiteSpace(serviceDto.Description) ? existingEntity.Description : serviceDto.Description;
            existingEntity.Duration = serviceDto.Duration > 0 ? serviceDto.Duration : existingEntity.Duration;
            existingEntity.Price = serviceDto.Price > 0 ? serviceDto.Price : existingEntity.Price;

            var updatedService = await _serviceRepository.TransactionUpdateAsync(s => s.Id == serviceDto.Id, existingEntity);

            if (updatedService == null)
            {
                await _serviceRepository.RollBackTransactionAsync();
                return Result.Error("Could not update the service");
            }

            var saveResult = await _serviceRepository.SaveAsync();
            if (saveResult > 0)
            {
                await _serviceRepository.CommitTransactionAsync();
                return Result<ServiceDto>.OK(ServiceFactory.ToDto(updatedService));
            }

            await _serviceRepository.RollBackTransactionAsync();
            return Result.Error("Update failed, no changes were saved");
        }
        catch (Exception ex)
        {
            await _serviceRepository.RollBackTransactionAsync();
            Console.WriteLine($"An error occurred when updating the service: {ex.Message}{ex.StackTrace}");
            return Result.Error("There was an error when updating the service");
        }
    }
        

    public async Task<IResult> DeleteServiceAsync(int id)
    {
        await _serviceRepository.BeginTransactionAsync();
        try
        {
            var exists = await _serviceRepository.DoesEntityExistAsync(s => s.Id == id);
            if (!exists)
            {
                await _serviceRepository.RollBackTransactionAsync();
                Console.WriteLine("Service not found.");
                return Result.NotFound("Could not find the service");
            }

            var serviceResult = await _serviceRepository.RemoveAsync(s => s.Id == id);
            if (!serviceResult)
            {
                await _serviceRepository.RollBackTransactionAsync();
                return Result.Error("There was an error when removing service ");
            }

            var serviceSaved = await _serviceRepository.SaveAsync();
            if (serviceSaved > 0)
            {
                await _serviceRepository.CommitTransactionAsync();
                return Result.OK();
            }

            await _serviceRepository.RollBackTransactionAsync();
            return Result.Error("There was an error when saving");
        }
        catch (Exception ex)
        {
            await _serviceRepository.RollBackTransactionAsync();
            Debug.WriteLine($"An error occured when deleting service: {ex.Message} {ex.StackTrace}");
            return Result.Error("There was an error when deleting service");
        }
    }
}
  
