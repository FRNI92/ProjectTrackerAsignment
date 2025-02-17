using Data_Infrastructure.Contexts;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Runtime.CompilerServices;

namespace Data_Infrastructure.Repositories;

public class EmployeeRepository : BaseRepository<EmployeesEntity>, IEmployeeRepository
{
    private readonly DataContext _context;
    public EmployeeRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async override Task<IEnumerable<EmployeesEntity>> GetAllAsync()
    {
        var entities = await _context.Employees
           .Include(r => r.Role)
           .ToListAsync();
        return entities;
    }
}
