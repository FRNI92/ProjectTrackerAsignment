using Business.Services;
using Data.Contexts;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

//inmemory database 
builder.Services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("TestDB"));
builder.Services.AddScoped<RoleService>();


//builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("Data Source=localhost;Initial Catalog=sql_database;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"));
builder.Services.AddScoped<IRolesRepository, RolesRepository>();


//builder.Services.AddScoped<EmployeeService>();
//builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();


var app = builder.Build();
app.MapOpenApi();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.Run();
