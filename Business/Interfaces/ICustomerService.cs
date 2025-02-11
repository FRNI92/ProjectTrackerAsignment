using Business.Dtos;
using Data.Entities;
using System.Linq.Expressions;

namespace Business.Interfaces
{
    public interface ICustomerService
    {/// <summary>
    /// Task: detta är en asynkron exekvering.
    /// <CustomerDto> är vad som kommer returneras
    /// CreateCustomerAsync är methodnamnet. den "tål" bara en parameter(customerDto kan heta vad som. bara den är av CustomerDtoTypen)
    /// </summary>
    /// <param name="customerDto"></param>
    /// <returns></returns>
        Task<IResult> CreateCustomerAsync(CustomerDto customerDto);
        Task<IResult> GetAllCustomerAsync();

        Task<IResult> UpdateCustomerAsync(CustomerDto customerDto);

        Task<IResult> DeleteCustomerAsync(CustomerDto customerDto);
    }
}