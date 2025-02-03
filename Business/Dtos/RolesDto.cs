using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Business.Dtos;

public class RolesDto
{
    [Key]
    public int Id { get; set; } // Primärnyckel

    [Required]
    public string Name { get; set; } = null!; // Rollens namn, t.ex. "Admin", "User"

    public string? Description { get; set; } // Beskrivning av rollen (valfritt)

}
