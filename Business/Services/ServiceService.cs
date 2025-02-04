﻿using Business.Dtos;
using Business.Factories;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Business.Services;

public class ServiceService
{
    private readonly IServiceRepository _serviceRepository;

    public ServiceService(IServiceRepository servicerepository)
    {
        _serviceRepository = servicerepository;
    }
    public async Task<ServiceDto> CreateServiceAsync(ServiceDto serviceDto)
    {
        try
        {
            if (serviceDto == null)
                throw new ArgumentNullException(nameof(serviceDto), "Service data cannot be null.");

            bool exists = await _serviceRepository.DoesEntityExistAsync(s => s.Name == serviceDto.Name);
            if (exists)
                throw new InvalidOperationException("A Service with this Name already exists.");

            var newServiceEntity = ServiceFactory.ToEntity(serviceDto);
            var CreatedService = await _serviceRepository.CreateAsync(newServiceEntity);

            return ServiceFactory.ToDto(CreatedService);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error With CreateServiceAsync{ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<ServiceDto>> ReadServiceAsync()
    {
        try
        {
            var serviceList = await _serviceRepository.GetAllAsync();
            if (!serviceList.Any())
            {
                Console.WriteLine("No Services found.");
                return [];
            }

            var convertedList = serviceList.Select(ServiceFactory.ToDto);
            return convertedList;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured when Reading Services: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }


    public async Task<ServiceDto> UpdateServiceAsync(ServiceDto serviceDto)
    {
        try
        {
            if (serviceDto == null)
                throw new ArgumentNullException(nameof(serviceDto), "Service data cannot be null.");

            var existingEntity = await _serviceRepository.GetAsync(s => s.Id == serviceDto.Id);
            if (existingEntity == null)
            {
                throw new InvalidOperationException("Could not find the entity to update.");
            }
            else
            {
                ServiceFactory.UpdateEntity(existingEntity, serviceDto);
                await _serviceRepository.UpdateAsync(s => s.Id == serviceDto.Id, existingEntity);
                return ServiceFactory.ToDto(existingEntity);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred when updating the service: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    public async Task <bool> DeleteServiceEntity(int id)
    {
        try
        {
            var exists = await _serviceRepository.DoesEntityExistAsync(s => s.Id == id);

            if (!exists)
            {
                Console.WriteLine("Service not found.");
                return false;
            }
            else
            {
                return await _serviceRepository.DeleteAsync(s => s.Id == id);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured when deleting service: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }
}
  
