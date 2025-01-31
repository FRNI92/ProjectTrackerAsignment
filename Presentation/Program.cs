using Business.Interfaces;
using Business.Services;
using Data.Contexts;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation;
using System;

//controller
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("Data Source=localhost;Initial Catalog=sql_database;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"));
//builder.Services.AddScoped<ICustomerService, CustomerService>();
//builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

var app = builder.Build();
app.MapOpenApi();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.Run();

//controller


var serviceCollection = new ServiceCollection();//servicecollection kommer från MSoft DInjectionn

// Registrera DataContext och lägg till connection string. kopplar min serversträng till DataContextclassen
serviceCollection.AddDbContext<DataContext>(options =>
    options.UseSqlServer("Data Source=localhost;Initial Catalog=sql_database;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"));


serviceCollection.AddScoped<ICustomerRepository, CustomerRepository>();
serviceCollection.AddScoped<ICustomerService, CustomerService>();

serviceCollection.AddScoped<IProjectRepository, ProjectRepository>();//DI för rep och project
serviceCollection.AddScoped<IProjectService, ProjectService>();

// Bygg service provider. här bygger den det jag definierat i collection
var serviceProvider = serviceCollection.BuildServiceProvider();

// Hämta en instans av DataContext från service provider. det jag definierat i collection och byggt i builder anropas här
using var context = serviceProvider.GetRequiredService<DataContext>();

// Här kan du använda context för att interagera med databasen
Console.WriteLine("DataContext har konfigurerats!");

var projectService = serviceProvider.GetRequiredService<IProjectService>();
var menu = new Menu(projectService);

await menu.ShowMenuAsync();


