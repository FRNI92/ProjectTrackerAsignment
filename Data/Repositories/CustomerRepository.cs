using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class CustomerRepository : BaseRepository<CustomerEntity>, ICustomerRepository
{
    private readonly DataContext _context;

    public CustomerRepository(DataContext context) : base(context)
    {
    }

}
