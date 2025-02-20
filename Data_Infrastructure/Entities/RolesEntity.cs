using System.ComponentModel.DataAnnotations;
namespace Data_Infrastructure.Entities;

public class RolesEntity
{
    [Key]
    public int Id { get; set; } 

    [Required]
    public string Name { get; set; } = null!; 

    public string? Description { get; set; } 


    public ICollection<EmployeesEntity> Employees { get; set; } = new List<EmployeesEntity>();
}
