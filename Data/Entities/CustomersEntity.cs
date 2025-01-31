using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class CustomerEntity
{
    [Key]
    public int Id { get; set; } // Primärnyckel

    [Required]
    [MaxLength(100)] // Begränsar längden till 100 tecken
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(15)] // Begränsar telefonnumret till 15 tecken
    public string PhoneNumber { get; set; } = null!;

    public ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();

}
