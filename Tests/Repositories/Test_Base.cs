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
//the Test_Base simplifies test setup byt creating an in-memory database.
//"_context" can be used in derived test classes, so the dont need to do the configure the database
//Its InMemory, the tests does not affect eachother, no data is save permamently