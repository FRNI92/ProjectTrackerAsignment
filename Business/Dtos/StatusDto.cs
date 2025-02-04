using Data.Entities;

namespace Business.Dtos;

public class StatusDto
{
    public int Id { get; set; } // Primärnyckel
    public string Name { get; set; } = null!; // Namn för status (t.ex. "Avslutat")

}
