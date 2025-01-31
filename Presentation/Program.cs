using Business.Interfaces;
using Business.Services;
using Data.Contexts;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation;
using System;

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


