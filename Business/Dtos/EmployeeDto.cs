using System.ComponentModel.DataAnnotations;
namespace Business.Dtos;

public class EmployeeDto
{
    public int Id { get; set; } // Primärnyckel
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;//unique?
    public int RoleId { get; set; } // Foreign Key till RoleEntity [ForeignKey(nameof(RoleId))] EFC förstår men man kan förtydliga
    public string RoleName { get; set; } = null!;
}
