﻿using Business.Dtos;

namespace Business.Interfaces
{
    public interface IServiceService
    {
        Task<IResult> CreateServiceAsync(ServiceDto serviceDto);
        Task<IResult> DeleteServiceAsync(int id);
        Task<IResult> ReadServiceAsync();

        Task<IResult> ReadServiceByIdAsync(int serviceId);
        Task<IResult> UpdateServiceAsync(ServiceDto serviceDto);
    }
}