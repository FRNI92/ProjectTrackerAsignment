using Business.Dtos;
namespace Business.Interfaces;

public interface IProjectService
{
    Task<IResult> CreateProjectAsync(ProjectDto projectDto);//ska innehålla en metod som hanterar typen ProjectDto och returnerar en ProjectDto
    Task<IResult> GetAllProjectAsync();//den klarar av en IEnumerable projectDto också. typ såhär Task<IResult<IEnumerable<ProjectDto>>>
    Task<ProjectDto> UpdateProjectAsync(ProjectDto projectDto);// ska kunna hantera typen ProjectDto och returnera en ProjectDto
    Task DeleteProjectAsync(ProjectDto projectDto);//ska kunna hantera typen ProjectDto
}
