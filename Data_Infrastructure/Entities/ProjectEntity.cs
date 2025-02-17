using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data_Infrastructure.Entities;

public class ProjectEntity
{
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
    [ForeignKey("StatusId")]
    public StatusEntity? Status { get; set; } // Navigation property

    [Required]
    public int CustomerId { get; set; } // Foreign Key till CustomerEntity
    [ForeignKey("CustomerId")]
    public CustomersEntity? Customer { get; set; } // Navigation property

    [Required]
    public int ServiceId { get; set; } // Foreign Key till ServiceEntity
    [ForeignKey("ServiceId")]
    public ServiceEntity? Service { get; set; } // Navigation property

    [Required]
    public int EmployeeId { get; set; } // Foreign Key till EmployeeEntity
    [ForeignKey("EmployeeId")]
    public EmployeesEntity? Employee { get; set; } // projectmanager
}
