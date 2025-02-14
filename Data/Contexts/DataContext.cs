
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{

    public DbSet<CustomerEntity> Customers{ get; set; }
    public DbSet<EmployeesEntity> Employees { get; set; }
    public DbSet<RolesEntity> Roles { get; set; }
    public DbSet<ServiceEntity> Services { get; set; }
    public DbSet<StatusEntity> Statuses { get; set; }
    public DbSet<ProjectEntity> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Lägg till roller först
        modelBuilder.Entity<RolesEntity>().HasData(
            new RolesEntity { Id = 1, Name = "Admin", Description = "Full access" },
            new RolesEntity { Id = 2, Name = "User", Description = "Limited access" }
        );

        modelBuilder.Entity<StatusEntity>().HasData(
            new StatusEntity { Id = 1, Name = "Ej påbörjat" },
            new StatusEntity { Id = 2, Name = "Pågående" },
            new StatusEntity { Id = 3, Name = "Avslutat" }
        );

    modelBuilder.Entity<CustomerEntity>().HasData(
            new CustomerEntity { Id = 1, Name = "Company A", Email = "contact@companyA.com", PhoneNumber = "123456789" },
            new CustomerEntity { Id = 2, Name = "Company B", Email = "contact@companyB.com", PhoneNumber = "987654321" },
            new CustomerEntity { Id = 3, Name = "Company c", Email = "contact@companyC.com", PhoneNumber = "987988976" }
        );
    modelBuilder.Entity<ServiceEntity>().HasData(
            new ServiceEntity { Id = 1, Name = "IT Support", Description = "Technical support", Duration = 2, Price = 500 },
            new ServiceEntity { Id = 2, Name = "Consulting", Description = "Business consulting", Duration = 5, Price = 1500 }
        );
    modelBuilder.Entity<EmployeesEntity>().HasData(
            new EmployeesEntity { Id = 1, FirstName = "Anna", LastName = "Andersson", Email = "anna@company.com", RoleId = 1 },
            new EmployeesEntity { Id = 2, FirstName = "Johan", LastName = "Johansson", Email = "johan@company.com", RoleId = 2 },
            new EmployeesEntity { Id = 3, FirstName = "Maria", LastName = "Karlsson", Email = "Maria@company.com", RoleId = 1 }
        );

        modelBuilder.Entity<ProjectEntity>().HasData(
            new ProjectEntity { Id = 1,ProjectNumber = "P-101", Name = "Projekt A", Description = "Första projektet", StartDate = new DateTime(2024, 1, 1), StatusId = 1, CustomerId = 1, ServiceId = 1, EmployeeId = 1, },
            new ProjectEntity { Id = 2,ProjectNumber = "P-102", Name = "Projekt B", Description = "Andra projektet", StartDate = new DateTime(2024, 1, 1), StatusId = 2, CustomerId = 2, ServiceId = 2, EmployeeId = 2, }
        );
    }
}
