using System.ComponentModel.DataAnnotations;

namespace Business.Dtos;

public class CustomerDto
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}
