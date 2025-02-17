using System.ComponentModel.DataAnnotations;
namespace Data_Infrastructure.Entities;

public class RolesEntity
{
    [Key]
    public int Id { get; set; } // Primärnyckel

    [Required]
    public string Name { get; set; } = null!; // Rollens namn, t.ex. "Admin", "User"

    public string? Description { get; set; } // Beskrivning av rollen (valfritt)


    public ICollection<EmployeesEntity> Employees { get; set; } = new List<EmployeesEntity>();
}
