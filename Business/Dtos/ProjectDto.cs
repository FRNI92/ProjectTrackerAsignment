using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Business.Dtos;

public class ProjectDto
{//behöver inte ha kopplingarna som man har i entiteterna
    [Key]
    public int Id { get; set; }

    [Required]
    public string ProjectNumber { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; } // Beskrivning

    [Required]
    public DateTime StartDate { get; set; } // Startdatum

    public DateTime? EndDate { get; set; } // Slutdatum

    public decimal TotalPrice { get; set; }

    [Required]
    public int StatusId { get; set; } // Foreign Key till StatusEntity

    public string StatusName { get; set; }
    [Required]
    public int CustomerId { get; set; } // Foreign Key till CustomerEntity
    public string CustomerName { get; set; }  
    [Required]
    
    public int ServiceId { get; set; } // Foreign Key till ServiceEntity
    public string ServiceName { get; set; }
    public decimal ServicePrice { get; set; } // Nytt fält
    public decimal ServiceDuration { get; set; } // Nytt fält
    
    [Required]
    public int EmployeeId { get; set; } // Foreign Key till EmployeeEntity
    public string EmployeeName { get; set; }
}
