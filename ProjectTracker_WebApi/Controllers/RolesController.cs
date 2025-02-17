//using Business.Dtos;
//using Business.Models;
//using Business.Services;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace ProjectTracker_WebApi.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class RolesController(RoleService rolesService) : ControllerBase
//{

//    private readonly RoleService _rolesService = rolesService;

//    [HttpGet]
//    public async Task<ActionResult<IEnumerable<RolesDto>>> GetAllRoles()
//    {
//        var roles = await _rolesService.GetAllRolesAsync();
//        return Ok(roles);
//    }

//    [HttpGet("{id}")]
//    public async Task<ActionResult<RolesDto>> GetRoleById(int id)
//    {
//        var role = await _rolesService.GetRoleByIdAsync(id);
//        if (role == null)
//            return NotFound($"No role found with ID {id}");

//        return Ok(role);
//    }

//    [HttpPost]
//    public async Task<ActionResult<RolesDto>> CreateRole([FromBody] RolesDto newRole)
//    {
//        if (newRole == null)
//            return BadRequest("Role data is required.");

//        try
//        {
//            var createdRole = await _rolesService.CreateRolesAsync(newRole);
//            if (createdRole is Result<RolesDto> okResult)
//            {
//                var projects = okResult.Data;
//                // Om resultatet är OK, returnera det skapade objektet
//                return CreatedAtAction(nameof(GetRoleById), new { id = okResult.Data.Id }, okResult.Data.Name);
//            }
//            return BadRequest("was not able to create role");
//        }
//        catch (ArgumentException ex)  // Fångar specifika fel från service
//        {
//            return Conflict(new { ErrorMessage = ex.Message });
//        }
//        catch (Exception ex)  // Fångar oväntade fel
//        {
//            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
//        }
//    }

//    [HttpPut("{id}")]
//    public async Task<ActionResult<RolesDto>> UpdateRole(int id, [FromBody] RolesDto updatedRole)
//    {
//        if (updatedRole == null)
//            return BadRequest("Updated role data is required.");

//        try
//        {
//            var role = await _rolesService.UpdateRoleByIdAsync(id, updatedRole);
//            return Ok(role);
//        }
//        catch (KeyNotFoundException ex)
//        {
//            return NotFound(new { ErrorMessage = ex.Message });  // Hanterar felet här
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
//            Console.WriteLine(ex.StackTrace);
//            return StatusCode(500, "An unexpected error occurred.");
//        }
//    }

//    [HttpDelete("{id}")]
//    public async Task<ActionResult> DeleteRole(int id)
//    {
//        var isDeleted = await _rolesService.DeleteRoleByIdAsync(id);
//        if (!isDeleted)
//            return NotFound($"No role found with ID {id}");

//        return NoContent();
//    }
//}
