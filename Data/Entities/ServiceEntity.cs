using System.ComponentModel.DataAnnotations;
namespace Data.Entities;

public class ServiceEntity
{
    [Key]
    public int Id { get; set; } // Primärnyckel

    [Required]
    public string Name { get; set; } = null!; // Namn på tjänsten

    public string? Description { get; set; } // Beskrivning av tjänsten

    public decimal Duration { get; set; } // Antal timmar för tjänsten

    public decimal Price { get; set; } // Pris på tjänsten


    public ICollection<ProjectEntity> Projects { get; set; } = [];
}
