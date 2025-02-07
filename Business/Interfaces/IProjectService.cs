using Business.Dtos;
namespace Business.Interfaces;

public interface IProjectService
{
    Task<IResult> CreateProjectAsync(ProjectDto projectDto);//ska innehålla en metod som hanterar typen ProjectDto och returnerar en ProjectDto
    Task<IEnumerable<ProjectDto>> GetAllProjectAsync();//ska returnera en Oskrivbar lista och object av typen ProjectDto
    Task<ProjectDto> UpdateProjectAsync(ProjectDto projectDto);// ska kunna hantera typen ProjectDto och returnera en ProjectDto
    Task DeleteProjectAsync(ProjectDto projectDto);//ska kunna hantera typen ProjectDto
}
