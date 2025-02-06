using Business.Dtos;
using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProjectTracker_WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatusController(StatusService statusService) : ControllerBase
{
    private readonly StatusService _statusService = statusService;

    [HttpPost]
    public async Task<ActionResult<StatusDto>> CreateStatus([FromBody] StatusDto newStatus)
    {
        if (newStatus == null)
            return BadRequest("Status data is required.");

        try
        {
            var createdStatus = await _statusService.CreateStatusAsync(newStatus);
            return Created("", createdStatus);
        }
        catch (ArgumentException ex)  // Fångar specifika fel från service
        {
            return Conflict(new { ErrorMessage = ex.Message });
        }
        catch (Exception ex)  // Fångar oväntade fel
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StatusDto>>> GetAllStatuses()
    {
        var status = await _statusService.ReadStatusAsync();
        return Ok(status);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StatusDto>> UpdateRole(int id, [FromBody] StatusDto updatedStatus)
    {
        if (updatedStatus == null)
            return BadRequest("Updated status data is required.");

        try
        {
            var status = await _statusService.UpdateStatusAsync(updatedStatus);
            return Ok(status);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ErrorMessage = ex.Message });  // Hanterar felet här
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteStatus(int id)
    {
        var isDeleted = await _statusService.DeleteStatusAsync(id);
        if (!isDeleted)
            return NotFound($"No status found with ID {id}");

        return NoContent();
    }
}

