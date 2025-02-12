using Business.Interfaces;
using Business.Services;
using Data.Contexts;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation;
using Presentation.MenuDialogs;
using System;

//controller
//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddControllers();
//builder.Services.AddOpenApi();

//builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("Data Source=localhost;Initial Catalog=sql_database;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"));


//builder.Services.AddScoped<EmployeeService>();
//builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

//var app = builder.Build();
//app.MapOpenApi();
//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();
//app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
//app.Run();

//controller
//builder.Services.AddScoped<ICustomerService, CustomerService>();
//builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

var serviceCollection = new ServiceCollection();//servicecollection kommer från MSoft DInjectionn

// Registrera DataContext och lägg till connection string. kopplar min serversträng till DataContextclassen
serviceCollection.AddDbContext<DataContext>(options =>
    options.UseSqlServer("Data Source=localhost;Initial Catalog=sql_database;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"));


serviceCollection.AddScoped<ICustomerRepository, CustomerRepository>();
serviceCollection.AddScoped<ICustomerService, CustomerService>(); // Här registreras CustomerService
serviceCollection.AddScoped<IProjectRepository, ProjectRepository>();
serviceCollection.AddScoped<IProjectService, ProjectService>();
serviceCollection.AddScoped<ProjectService>();
serviceCollection.AddScoped<IEmployeeService, EmployeeService>();
serviceCollection.AddScoped<IEmployeeRepository, EmployeeRepository>();
serviceCollection.AddScoped<IRoleService, RoleService>();
serviceCollection.AddScoped<IRolesRepository, RolesRepository>();
serviceCollection.AddScoped<StatusService>();
serviceCollection.AddScoped<IStatusRepository, StatusRepository>();
serviceCollection.AddScoped<IServiceRepository, ServiceRepository>();
serviceCollection.AddScoped<ServiceService>();

// Registrera dina meny-dialoger
serviceCollection.AddScoped<ProjectMenuDialogs>();
serviceCollection.AddScoped<CustomerMenuDialogs>();
serviceCollection.AddScoped<EmployeeMenuDialogs>();
serviceCollection.AddScoped<RoleMenuDialog>();
serviceCollection.AddScoped<StatusMenuDiallogs>();
serviceCollection.AddScoped<ServiceMenuDialogs>();
serviceCollection.AddScoped<MainMenu>();  // Registrera MainMenu

// Bygg service provider
var serviceProvider = serviceCollection.BuildServiceProvider();

// Hämta en instans av MainMenu från service provider
var mainMenu = serviceProvider.GetRequiredService<MainMenu>();
await mainMenu.ShowMainMenuAsync();



