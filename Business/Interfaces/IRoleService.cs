using Business.Dtos;

namespace Business.Interfaces;

public interface IRoleService
{
    public Task<IResult> CreateRoleAsync(RolesDto roleDto);
    public Task<IResult> GetAllRolesAsync();

    public Task<IResult> UpdateRolesAsync(RolesDto roleDto);

    public Task<IResult> DeleteRolesAsync(RolesDto rolesDto);
}
