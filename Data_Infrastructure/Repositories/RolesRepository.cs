using Data_Infrastructure.Contexts;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;

namespace Data_Infrastructure.Repositories;

public class RolesRepository : BaseRepository<RolesEntity>, IRolesRepository
{
    private readonly DataContext _datacontext;

    public RolesRepository(DataContext context) : base(context)
    {
        _datacontext = context;
    }
}
