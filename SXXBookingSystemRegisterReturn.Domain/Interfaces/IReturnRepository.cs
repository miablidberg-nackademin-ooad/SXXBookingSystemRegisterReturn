using SXXBookingSystemRegisterReturn.Domain.Models;

namespace SXXBookingSystemRegisterReturn.Domain.Interfaces;

public interface IReturnRepository
{
    public Task<bool> AddAsync(Return returnToAdd);
}
