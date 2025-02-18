using Data_Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories;

public abstract class Test_Base
{
    protected readonly DataContext _context;

    protected Test_Base()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Skapar unik testdatabas
            .Options;

        _context = new DataContext(options);
    }
}
