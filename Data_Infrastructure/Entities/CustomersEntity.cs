using System.ComponentModel.DataAnnotations;

namespace Data_Infrastructure.Entities;

public class CustomersEntity
{
    [Key]
    public int Id { get; set; } 
    [Required]
    [MaxLength(100)] 
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(15)] 
    public string PhoneNumber { get; set; } = null!;

    public ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();

}
