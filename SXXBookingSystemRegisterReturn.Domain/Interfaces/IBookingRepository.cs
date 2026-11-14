using SXXBookingSystemRegisterReturn.Domain.Models;

namespace SXXBookingSystemRegisterReturn.Domain.Interfaces;

public interface IBookingRepository
{
    public Task<Booking?> GetByIdAsync(Guid id);
    public Task<bool> UpdateAsync(Booking booking);
}
