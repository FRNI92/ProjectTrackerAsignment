using Data_Infrastructure.Contexts;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data_Infrastructure.Repositories;

public class ProjectRepository : BaseRepository<ProjectEntity>, IProjectRepository
{
    private readonly DataContext _context;
    public ProjectRepository(DataContext context) : base(context)
    {
        _context = context; 
        }

    public async override Task<IEnumerable<ProjectEntity>> GetAllAsync()
    {
        var entities = await _context.Projects
            .Include(s => s.Status)
            .Include(p => p.Employee)
            .Include(p => p.Customer)
            .Include(p => p.Service)
            .ToListAsync();
        return entities;
 
    }

    public async Task<ServiceEntity?> GetServiceByIdAsync(int serviceId)
    {
        return await _context.Services.FirstOrDefaultAsync(s => s.Id == serviceId);
    }

}

