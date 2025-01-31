using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class RolesRepository : BaseRepository<RolesEntity>, IRolesRepository
{
    private readonly DataContext _datacontext;

    public RolesRepository(DataContext context) : base(context)
    {
        _datacontext = context;
    }
}
