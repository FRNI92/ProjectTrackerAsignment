using Data_Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data_Infrastructure.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{

    public DbSet<CustomersEntity> Customers{ get; set; }
    public DbSet<EmployeesEntity> Employees { get; set; }
    public DbSet<RolesEntity> Roles { get; set; }
    public DbSet<ServiceEntity> Services { get; set; }
    public DbSet<StatusEntity> Statuses { get; set; }
    public DbSet<ProjectEntity> Projects { get; set; }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StatusEntity>().HasData(
            new StatusEntity { Id = 1, Name = "Ej påbörjat" },
            new StatusEntity { Id = 2, Name = "Pågående" },
            new StatusEntity { Id = 3, Name = "Avslutat" }
        );

        modelBuilder.Entity<RolesEntity>().HasData(
            new RolesEntity { Id = 1, Name = "Admin", Description = "Administrator role with full permissions" },
            new RolesEntity { Id = 2, Name = "User", Description = "Regular user role with limited permissions" },
            new RolesEntity { Id = 3, Name = "Manager", Description = "Role for project managers" }
        );


        modelBuilder.Entity<CustomersEntity>().HasData(
            new CustomersEntity { Id = 1, Name = "Company A", Email = "contact@companyA.com", PhoneNumber = "123456789" },
            new CustomersEntity { Id = 2, Name = "Company B", Email = "contact@companyB.com", PhoneNumber = "987654321" }
        );


        modelBuilder.Entity<ServiceEntity>().HasData(
            new ServiceEntity { Id = 1, Name = "Consulting", Description = "Advice and feedback", Duration = 10, Price = 1000 },
            new ServiceEntity { Id = 2, Name = "IT-Support", Description = "Hardware and software", Duration = 1, Price = 500 }
        );

        modelBuilder.Entity<EmployeesEntity>().HasData(
            new EmployeesEntity { Id = 1, FirstName = "Fredrik", LastName = "Nilsson",Email = "Fredrik@domain.com", RoleId = 1 },
            new EmployeesEntity { Id = 2, FirstName = "Hans", LastName = "Andersson", Email = "Hans@domain.com", RoleId = 2 }
        );

        modelBuilder.Entity<ProjectEntity>().HasData(
            new ProjectEntity
            {
                Id = 1,
                ProjectNumber = "P-101",
                Name = "Projekt A",
                Description = "Första projektet",
                StartDate = new DateTime(2024, 1, 1),  // Använd statiskt datum
                EndDate = new DateTime(2024, 9, 1),
                StatusId = 1,  // Ej påbörjat
                CustomerId = 1,  // Company A
                ServiceId = 1,  // Consulting
                EmployeeId = 1,  // Fredrik Nilsson
                Duration = 10,
                //TotalPrice = 5000 räknas ut av servicen endå
            }
        );
    }

}
