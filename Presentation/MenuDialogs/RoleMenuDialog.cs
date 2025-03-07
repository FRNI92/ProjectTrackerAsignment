﻿using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Rewrite;
using Presentation.Interfaces;
using System;


namespace Presentation.MenuDialogs;

public class RoleMenuDialog : IRoleMenuDialog
{
    private readonly IRoleService _roleService;

    public RoleMenuDialog(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task ShowRolesMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Show All Roles");
            Console.WriteLine("2. Create a new Role");
            Console.WriteLine("3. See Details And Update Roles");
            Console.WriteLine("4. Delete a Role");
            Console.WriteLine("0. Go Back To Main Menu");

            Console.Write("\nChoose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ShowRolesAsync();
                    break;
                case "2":
                    await CreateRoleAsync();
                    break;
                case "3":
                    await UpdateRoleAsync();
                    break;
                case "4":
                    await DeleteRoleAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }


    private async Task ShowRolesAsync()
    {
        var roles = await _roleService.GetAllRolesAsync();
        if (roles is Result<IEnumerable<RolesDto>> roleResult && roleResult.Success)
        {
            var rolesData = roleResult.Data;
            foreach (var role in rolesData)
            {

                Console.WriteLine($" Name: {role.Name}, Role Description: {role.Description}");
            }
            Console.WriteLine(roles.StatusCode);
        }
        else
        {
            Console.WriteLine($"{roles.ErrorMessage} {roles.StatusCode}");
        }
        Console.ReadKey();
    }


    private async Task CreateRoleAsync()
    {

        var newRole = new RolesDto();

        Console.Write("Enter Role name: ");
        newRole.Name = Console.ReadLine()!;

        Console.Write("Enter Role description: ");
        newRole.Description = Console.ReadLine()!;


        var createdNewRole = await _roleService.CreateRoleAsync(newRole);
        if (createdNewRole.Success)
        {
            Console.WriteLine($"Role created: {newRole.Name}");
        }
        else
        {
            Console.WriteLine($"Error: {createdNewRole.ErrorMessage}");
        }
        Console.ReadKey();
    }

    private async Task UpdateRoleAsync()
    {
        Console.Clear();
        Console.WriteLine("This Is The Role-List:");

        var roles = await _roleService.GetAllRolesAsync();

        if (roles is Result<IEnumerable<RolesDto>> rolesResult && roles.Success)
        {
            var rolesData = rolesResult.Data;

            if (!rolesData.Any())
            {
                Console.WriteLine("No roles found.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            int index = 1;
            foreach (var roledata in rolesData)
            {
                Console.WriteLine($"{index}.Role Name: {roledata.Name}\t {roledata.Description}\n");
                Console.WriteLine("------------------------------------");
                index++;
            }

            Console.WriteLine("----Which Role Would You Like To Update? Enter the number:");


            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= rolesData.Count())
            {
                var selectedRole = rolesData.ElementAt(choice - 1);

                Console.WriteLine($"Enter new name (leave blank to keep current: {selectedRole.Name}):");
                var newName = Console.ReadLine();

                Console.WriteLine($"Enter new desicription (leave blank to keep current: {selectedRole.Description}):");
                var newDescription = Console.ReadLine();

                var updatedRole = new RolesDto()
                {
                    Id = selectedRole.Id,
                    Name = string.IsNullOrWhiteSpace(newName) ? selectedRole.Name : newName,
                    Description = string.IsNullOrWhiteSpace(newDescription) ? selectedRole.Description : newDescription,
                };

                Console.WriteLine("\nDo you want to save changes?");
                Console.Write("Type 'yes' to save, or anything else to cancel: ");
                var confirmSave = Console.ReadLine();

                if (confirmSave?.ToLower() != "yes")
                {
                    Console.WriteLine("\nEdit cancelled. Returning to menu...");
                    Console.ReadKey();
                    return;
                }

                var result = await _roleService.UpdateRolesAsync(updatedRole);
                if (result is Result<RolesDto> updateResult && updateResult.Success)
                {
                    Console.WriteLine("Role updated successfully!");
                }
                else
                {
                    Console.WriteLine($"Error: {result.ErrorMessage}");
                }
                Console.ReadKey();
            }
        }
    }

    private async Task DeleteRoleAsync()
    {
        Console.Clear();
        Console.WriteLine("ROLE-MANAGER");
        Console.WriteLine("\tDelete Role");

        Console.WriteLine("This Is The Role-List:");
        var rolesResult = await _roleService.GetAllRolesAsync();
        if (rolesResult is not Result<IEnumerable<RolesDto>> roleResult || !roleResult.Success)
        {
            Console.WriteLine("Failed to load roles or no roles available.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        var roles = roleResult.Data.ToList();
        if (!roles.Any())
        {
            Console.WriteLine("No roles found.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        int index = 1;
        foreach (var role in roles)
        {
            Console.WriteLine($"{index}. Name: {role.Name}, Description: {role.Description}");
            index++;
        }

        Console.WriteLine("----Which Role Would You Like To Delete?");
        if (!int.TryParse(Console.ReadLine(), out int choice) || choice <= 0 || choice > roles.Count())
        {
            Console.WriteLine("Invalid choice.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        var selectedRole = roles.ElementAt(choice - 1);

        Console.WriteLine($"Are you sure you want to delete this Role: {selectedRole.Name}? (y/n)");
        if (Console.ReadLine()?.ToLower() != "y")
        {
            Console.WriteLine("Role deletion cancelled.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        try
        {
            await _roleService.DeleteRolesAsync(selectedRole);
            Console.WriteLine("Role deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete the role. Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}

