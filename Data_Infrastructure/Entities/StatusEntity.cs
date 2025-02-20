using System.ComponentModel.DataAnnotations;

namespace Data_Infrastructure.Entities;

public class StatusEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = null!; 

    /// <summary>
    /// defines 1 to many relation between Status and Projects. tells EFC how to connect entities
    /// </summary>
    public ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();
}
