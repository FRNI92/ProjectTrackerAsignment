using Business.Dtos;

namespace Business.Interfaces
{
    public interface IStatusService
    {
        Task<IResult> CreateStatusAsync(StatusDto statusDto);
        Task<IResult> DeleteStatusAsync(int id);
        Task<IResult> ReadStatusAsync();
        Task<IResult> UpdateStatusAsync(StatusDto statusDto);
    }
}