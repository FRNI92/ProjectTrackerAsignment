using System.ComponentModel.DataAnnotations;

namespace Business.Dtos;

public class RolesDto
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

}
