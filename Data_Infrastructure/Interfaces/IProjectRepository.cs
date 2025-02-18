using Data_Infrastructure.Entities;

namespace Data_Infrastructure.Interfaces;

public interface IProjectRepository : IBaseRepository<ProjectEntity>
{
    //Task<ServiceEntity?> GetServiceByIdAsync(int serviceId);
}
