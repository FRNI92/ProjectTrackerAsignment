using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data_Infrastructure.Entities;

public class EmployeesEntity
{
    [Key]
    public int Id { get; set; } 

    [Required]
    public string FirstName { get; set; } = null!; 

    [Required]
    public string LastName { get; set; } = null!; 

    public string Email { get; set; } = null!;

    public int RoleId { get; set; } 
    public RolesEntity? Role { get; set; } 
}
