using System.ComponentModel.DataAnnotations;
namespace Business.Dtos;

public class ServiceDto
{
    [Key]
    public int Id { get; set; } // Primärnyckel

    [Required]
    public string Name { get; set; } = null!; 

    public string? Description { get; set; } 

    public decimal Duration { get; set; }

    public decimal Price { get; set; }
}
