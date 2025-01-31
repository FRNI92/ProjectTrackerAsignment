using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=sql_database;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

        return new DataContext(optionsBuilder.Options);
    }
}
