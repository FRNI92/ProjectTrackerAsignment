using Data_Infrastructure.Contexts;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;

namespace Data_Infrastructure.Repositories;

public class ServiceRepository : BaseRepository<ServiceEntity>, IServiceRepository
{
    private readonly DataContext _context;

    public ServiceRepository(DataContext context) : base(context)
    {
        _context = context;
    }
}

