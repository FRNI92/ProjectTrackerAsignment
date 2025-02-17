using Data_Infrastructure.Contexts;
using Data_Infrastructure.Entities;
using Data_Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data_Infrastructure.Repositories;

public class CustomerRepository : BaseRepository<CustomersEntity>, ICustomerRepository
{
    private readonly DataContext _context;

    public CustomerRepository(DataContext context) : base(context)
    {
    }

}
