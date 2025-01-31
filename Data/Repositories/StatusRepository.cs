using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories;

public class StatusRepository : BaseRepository<StatusEntity>, IStatusRepository
{
    private readonly DataContext _context;

    public StatusRepository(DataContext context) : base(context)
    {
        _context = context;
    }
}
