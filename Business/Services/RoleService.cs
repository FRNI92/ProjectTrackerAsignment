using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics;
using System.Linq.Expressions;
namespace Business.Services;

public class RoleService(IRolesRepository rolesRepository) : IRoleService
{
    private readonly IRolesRepository _rolesRepository = rolesRepository;

    public async Task <IResult> CreateRoleAsync(RolesDto RolesDto)
    {
        try
        {
            var exists = await _rolesRepository.DoesEntityExistAsync(r => r.Name == RolesDto.Name);
            if (exists)
            {
                return Result.AlreadyExists("A role with this name already exists.");
            }

            var newEntity = RoleFactory.ToEntity(RolesDto);

            var success = await _rolesRepository.CreateAsync(newEntity);

            if (success)
            {
                return Result<RolesDto>.OK(); 
            }

            return Result.Error("Failed to create role");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred when creating role: {ex.Message}");
            return Result.BadRequest("Could not create role");
        }
    }

    public async Task <IResult> GetAllRolesAsync()
    {
        try
        {
            var rolesList = await _rolesRepository.GetAllAsync();

            if (!rolesList.Any())
                Console.WriteLine("There are no Roles here");
            var rolesDto = rolesList.Select(RoleFactory.ToDto);
            return Result<IEnumerable<RolesDto>>.OK(rolesDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error when Getting Roles:{ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return Result.NotFound("could not find any roles");
        }
    }

    public async Task<IResult> UpdateRolesAsync(RolesDto updatedRolesDto)
    {
        if (updatedRolesDto == null)
            return Result.BadRequest("Updated role data cannot be null.");

            try
            {
                var existingRole = await _rolesRepository.GetAsync(r => r.Id == updatedRolesDto.Id);
                if (existingRole == null)
                    return Result.NotFound($"Could not find a role with that id {updatedRolesDto.Id}");

                RoleFactory.UpdateEntity(existingRole, updatedRolesDto);

                var result = await _rolesRepository.UpdateAsync(r => r.Id == updatedRolesDto.Id, existingRole);
                return result != null
                ? Result<RolesDto>.OK(RoleFactory.ToDto(existingRole))
                : Result.Error("Unable to update role");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred when updating roles: {ex.Message}{ex.StackTrace}");
                return Result.Error("There was an error with updating roles");
            }
    }

    public async Task <IResult> DeleteRolesAsync(RolesDto rolesDto)
    {
        try
        {
            var exists = await _rolesRepository.DoesEntityExistAsync(r => r.Id == rolesDto.Id);

            if (!exists)
            {
                return Result.NotFound("Could not find that role");
            }
            var result = await _rolesRepository.DeleteAsync(r => r.Id == rolesDto.Id);

            if (result)
            {
                return Result.OK();
            }

            return Result.Error("Failed to delete role");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred when deleting role: {ex.Message} {ex.StackTrace}");
            return Result.Error("There was an error with deleting that role");
        }
    }
}
