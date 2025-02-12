using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics;

namespace Business.Services;

public class ServiceService
{
    private readonly IServiceRepository _serviceRepository;

    public ServiceService(IServiceRepository servicerepository)
    {
        _serviceRepository = servicerepository;
    }
    public async Task<IResult> CreateServiceAsync(ServiceDto serviceDto)
    {
        try
        {
            if (serviceDto == null)
                return Result.BadRequest("The service dto was not filled correctly");

            bool exists = await _serviceRepository.DoesEntityExistAsync(s => s.Name == serviceDto.Name);
            if (exists)
                return Result.AlreadyExists("Service with that name already exists");

            var newServiceEntity = ServiceFactory.ToEntity(serviceDto);
            var CreatedService = await _serviceRepository.CreateAsync(newServiceEntity);

            return Result.OK();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error With CreateServiceAsync{ex.Message}{ex.StackTrace}");
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


    public async Task<IResult> UpdateServiceAsync(ServiceDto serviceDto)
    {
        try
        {
            if (serviceDto == null)
                return Result.BadRequest("serviceDto cant be empty");

            var existingEntity = await _serviceRepository.GetAsync(s => s.Id == serviceDto.Id);
            if (existingEntity == null)
            {
                return Result.NotFound("Could not find the service to update");
            }
            else
            {
                ServiceFactory.UpdateEntity(existingEntity, serviceDto);
                var updatedService = await _serviceRepository.UpdateAsync(s => s.Id == serviceDto.Id, existingEntity);
                var servuceToMenu = ServiceFactory.ToDto(updatedService);
                return Result<ServiceDto>.OK(servuceToMenu);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred when updating the service: {ex.Message}{ex.StackTrace}");
            return Result.Error("There was an error when updating servicec");
        }
    }

    public async Task <IResult> DeleteServiceEntity(int id)
    {
        try
        {
            var exists = await _serviceRepository.DoesEntityExistAsync(s => s.Id == id);

            if (!exists)
            {
                Console.WriteLine("Service not found.");
                return Result.NotFound("Could not find the service");
            }
            else
            {
                var serviceResult = await _serviceRepository.DeleteAsync(s => s.Id == id);
                return Result.OK();
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
  
