using Data_Infrastructure.Contexts;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data_Infrastructure.Repositories;

public class StatusRepository : BaseRepository<StatusEntity>, IStatusRepository
{
    private readonly DataContext _context;

    public StatusRepository(DataContext context) : base(context)
    {
        _context = context;
    }
}
