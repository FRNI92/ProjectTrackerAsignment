using System.ComponentModel.DataAnnotations;
namespace Data_Infrastructure.Entities;

public class ServiceEntity
{
    [Key]
    public int Id { get; set; } 
    [Required]
    public string Name { get; set; } = null!; 

    public string? Description { get; set; } 

    public decimal Duration { get; set; } 

    public decimal Price { get; set; } 


    public ICollection<ProjectEntity> Projects { get; set; } = [];
}
