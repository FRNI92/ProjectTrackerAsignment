using Business.Dtos;

using System.Linq.Expressions;

namespace Business.Interfaces;

public interface ICustomerService
{/// <summary>
/// Task: detta är en asynkron exekvering.
/// <CustomerDto> är vad som kommer returneras
/// CreateCustomerAsync är methodnamnet. den "tål" bara en parameter(customerDto kan heta vad som. bara den är av CustomerDtoTypen)
/// </summary>
/// <param name="customerDto"></param>
/// <returns></returns>
    public Task<IResult> CreateCustomerAsync(CustomerDto customerDto);
    public Task<IResult> GetAllCustomerAsync();

    public Task<IResult> UpdateCustomerAsync(CustomerDto customerDto);

    public Task<IResult> DeleteCustomerAsync(CustomerDto customerDto);
}