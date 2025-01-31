using Data.Entities;
using System.ComponentModel.DataAnnotations;
namespace Business.Dtos;

public class EmployeeDto
{
    public int Id { get; set; } // Primärnyckel
    [Required]
    public string FirstName { get; set; } = null!; // Förnamn
    [Required]
    public string LastName { get; set; } = null!; // Efternamn
    public string Email { get; set; } = null!;//unique?
    public int RoleId { get; set; } // Foreign Key till RoleEntity [ForeignKey(nameof(RoleId))] EFC förstår men man kan förtydliga

}
