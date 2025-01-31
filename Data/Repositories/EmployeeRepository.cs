using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class EmployeeRepository : BaseRepository<EmployeesEntity>, IEmployeeRepository
{
    private readonly DataContext _context;
    public EmployeeRepository(DataContext context) : base(context)
    {
        _context = context;
    }
}
