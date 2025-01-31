
using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class ProjectForm
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; } // Beskrivning

    [Required]
    public DateTime StartDate { get; set; } // Startdatum

    public DateTime? EndDate { get; set; } // Slutdatum

    

    public string StatusName { get; set; } = null!; // Namn för status (t.ex. "Avslutat")
    public string CustomerName { get; set; } = null!;
}
