using SXXBookingSystemRegisterReturn.Domain.Models;

namespace SXXBookingSystemRegisterReturn.Domain.Interfaces;

internal interface IBookingRepository
{
    public Booking GetByIdAsync(Guid id);
    public Task<bool> UpdateAsync(Booking booking);
}
