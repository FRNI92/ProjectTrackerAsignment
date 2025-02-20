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

    public string? Description { get; set; } 

    [Required]
    public DateTime StartDate { get; set; } 

    public DateTime? EndDate { get; set; } 

    public decimal TotalPrice { get; set; }
    public decimal Duration { get; set; }

    [Required]
    public int StatusId { get; set; } // Foreign Key till StatusEntity

    public string StatusName { get; set; }
    [Required]
    public int CustomerId { get; set; } // Foreign Key till CustomerEntity
    public string CustomerName { get; set; }  
    [Required]
    
    public int ServiceId { get; set; } // Foreign Key till ServiceEntity
    public string ServiceName { get; set; }
    
    [Required]
    public int EmployeeId { get; set; } // Foreign Key till EmployeeEntity
    public string EmployeeName { get; set; }
}
