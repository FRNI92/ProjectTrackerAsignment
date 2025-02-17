using System.ComponentModel.DataAnnotations;

namespace Business.Dtos;

public class StatusDto
{
    [Key]
    public int Id { get; set; } // Primärnyckel
    public string Name { get; set; } = null!; // Namn för status (t.ex. "Avslutat")

}
