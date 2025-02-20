using Business.Dtos;
namespace Business.Interfaces;

public interface IProjectService
{
    Task<IResult> CreateProjectAsync(ProjectDto projectDto);
    Task<IResult> GetAllProjectAsync();
    Task<IResult> UpdateProjectAsync(ProjectDto projectDto);
    Task<IResult> DeleteProjectAsync(ProjectDto projectDto);
}
