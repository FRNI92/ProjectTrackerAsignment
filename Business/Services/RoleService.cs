using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data_Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics;
using System.Linq.Expressions;
namespace Business.Services;

public class RoleService(IRolesRepository rolesRepository) : IRoleService
{
    private readonly IRolesRepository _rolesRepository = rolesRepository;

    public async Task <IResult> CreateRoleAsync(RolesDto rolesDto)
    {
        if (rolesDto == null)
            return Result.BadRequest("RolesDto cannot be null");

        
        await _rolesRepository.BeginTransactionAsync();
        try
        {
            var exists = await _rolesRepository.DoesEntityExistAsync(r => r.Name == rolesDto.Name);
            if (exists)
            {
                await _rolesRepository.RollBackTransactionAsync();
                return Result.AlreadyExists("A role with this name already exists.");
            }

            var newEntity = RoleFactory.ToEntity(rolesDto);

            var addedEntity = await _rolesRepository.AddAsync(newEntity);
            if (addedEntity == false)
            {
                await _rolesRepository.RollBackTransactionAsync();
                return Result.Error("Could not add role");
            }
            var successSaved = await _rolesRepository.SaveAsync();
            if (successSaved > 0)
            {
                await _rolesRepository.CommitTransactionAsync();
                return Result<RolesDto>.OK(RoleFactory.ToDto(newEntity));
            }

            await _rolesRepository.RollBackTransactionAsync();
            return Result.Error("Failed to create role");
        }
        catch (Exception ex)
        {
            await _rolesRepository.RollBackTransactionAsync();
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
                return Result.NotFound("There are no Roles here");

            var rolesDto = rolesList.Select(RoleFactory.ToDto);
            return Result<IEnumerable<RolesDto>>.OK(rolesDto, "Roles Fetched successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error when Getting Roles:{ex.Message}{ex.StackTrace}");
            return Result.Error("Error when getting Roles");
        }
    }

    public async Task<IResult> UpdateRolesAsync(RolesDto updatedRolesDto)
    {
        if (updatedRolesDto == null)
            return Result.BadRequest("Updated role data cannot be null.");

        await _rolesRepository.BeginTransactionAsync();
        try
        {
            var existingRole = await _rolesRepository.GetAsync(r => r.Id == updatedRolesDto.Id);
            if (existingRole == null)
            {
                await _rolesRepository.RollBackTransactionAsync();
                return Result.NotFound($"Could not find a role with that id {updatedRolesDto.Id}");
            }

            var updatedEntity = RoleFactory.ToEntity(updatedRolesDto);
            var result = await _rolesRepository.TransactionUpdateAsync(r => r.Id == updatedRolesDto.Id, updatedEntity);

            if (result == null)
            {
                await _rolesRepository.RollBackTransactionAsync();
                return Result.Error("Unable to update role");
            }

            // Save changes to database
            var saveResult = await _rolesRepository.SaveAsync();
            if (saveResult > 0)
            {
                await _rolesRepository.CommitTransactionAsync();
                return Result<RolesDto>.OK(RoleFactory.ToDto(existingRole));
            }

            await _rolesRepository.RollBackTransactionAsync();
            return Result.Error("Update failed, no changes were saved");
        }
        catch (Exception ex)
        {
            await _rolesRepository.RollBackTransactionAsync();
            Debug.WriteLine($"An error occurred when updating roles: {ex.Message}{ex.StackTrace}");
            return Result.Error("There was an error with updating roles");
        }
    }

    public async Task <IResult> DeleteRolesAsync(RolesDto rolesDto)
    {
        if (rolesDto == null)
            return Result.BadRequest("rolesdto cant be null"); ;

        await _rolesRepository.BeginTransactionAsync();
        try
        {
            var exists = await _rolesRepository.DoesEntityExistAsync(r => r.Id == rolesDto.Id);

            if (!exists)
            {
                await _rolesRepository.RollBackTransactionAsync();
                return Result.NotFound("Could not find that role");
            }

            var result = await _rolesRepository.RemoveAsync(r => r.Id == rolesDto.Id);
            if (!result)
            {
                await _rolesRepository.RollBackTransactionAsync();
                return Result.Error("Failed to delete role");
            }
            var saveResult = await _rolesRepository.SaveAsync();
            if (saveResult > 0)
            {
                await _rolesRepository.CommitTransactionAsync();
                return Result.OK();
            }

            await _rolesRepository.RollBackTransactionAsync();
            return Result.Error("Deletion failed, no changes were saved");
        }
        catch (Exception ex)
        {
            await _rolesRepository.RollBackTransactionAsync();
            Debug.WriteLine($"An error occurred when deleting role: {ex.Message} {ex.StackTrace}");
            return Result.Error("There was an error with deleting that role");
        }
    }
}
