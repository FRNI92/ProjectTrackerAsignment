using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics;
using System.Linq.Expressions;
namespace Business.Services;

public class RoleService(IRolesRepository rolesRepository)
{
    private readonly IRolesRepository _rolesRepository = rolesRepository;

    public async Task <IResult> CreateRolesAsync(RolesDto RolesDto)
    {
        try
        {
            var exists = await _rolesRepository.DoesEntityExistAsync(r => r.Name == RolesDto.Name);
            if (exists)
            {
                return Result.Error("A role with this name already exists.");
            }

            var newEntity = RoleFactory.ToEntity(RolesDto);

            var success = await _rolesRepository.CreateAsync(newEntity);

            if (success)
            {
                return Result<RolesDto>.OK(); // Returnera OK-status
            }

            return Result.Error("Failed to create role"); // Om något gick fel
        }
        catch (Exception ex)
        {
            // Hantera fel och returnera ett felresultat
            Debug.WriteLine($"An error occurred when creating role: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return Result.BadRequest("Could not create role");
        }
    }

    public async Task <IEnumerable<RolesDto>> GetAllRolesAsync()
    {
        try
        {
            var rolesList = await _rolesRepository.GetAllAsync();

            if (!rolesList.Any())
                Console.WriteLine("There are no Roles here");

            return rolesList.Select(RoleFactory.ToDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error when Getting Roles:{ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    public async Task<RolesDto> GetRoleByIdAsync(int id)
    {
        try
        {
            var role = await _rolesRepository.GetAsync(r => r.Id == id);
            if (role == null)
                throw new KeyNotFoundException($"No role found with ID {id}.");

            return RoleFactory.ToDto(role);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occured when getting roles by Id{ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    public async Task<RolesDto> UpdateRoleByIdAsync(int id, RolesDto updatedRolesDto)
    {
        try
        {
            if (updatedRolesDto == null)
                throw new ArgumentNullException(nameof(updatedRolesDto), "Updated role data cannot be null.");


            var existingRole = await _rolesRepository.GetAsync(r => r.Id == id);
            if (existingRole == null)
                throw new KeyNotFoundException($"No role found with ID {id}.");

            RoleFactory.UpdateEntity(existingRole, updatedRolesDto);

            await _rolesRepository.UpdateAsync(r => r.Id == id, existingRole);
            return RoleFactory.ToDto(existingRole);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured when updating Roles{ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    public async Task <bool> DeleteRoleByIdAsync(int id)
    {
        try
        {
            var exists = await _rolesRepository.DoesEntityExistAsync(r => r.Id == id);

            if (!exists)
            {
                Console.WriteLine("Rollen hittades inte.");
                return false;
            }
            return await _rolesRepository.DeleteAsync(r => r.Id == id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured when deleting role: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }
}
