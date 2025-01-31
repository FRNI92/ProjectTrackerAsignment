using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class ServiceRepository : BaseRepository<ServiceEntity>, IServiceRepository
{
    private readonly DataContext _context;

    public ServiceRepository(DataContext context) : base(context)
    {
        _context = context;
    }
}

