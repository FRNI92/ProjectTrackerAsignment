namespace Data.Entities;

public class StatusEntity
{
    public int Id { get; set; } // Primärnyckel
    public string Name { get; set; } = null!; // Namn för status (t.ex. "Avslutat")

    /// <summary>
    /// defines 1 to many relation between Status and Projects. tells EFC how to connect entities
    /// </summary>
    public ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();
    // ICollection kommer från System.Collections.Generic
}
