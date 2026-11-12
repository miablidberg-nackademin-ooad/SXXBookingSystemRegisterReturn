using SXXBookingSystemRegisterReturn.Application.Dtos;
using SXXBookingSystemRegisterReturn.Domain.Models;

namespace SXXBookingSystemRegisterReturn.Application.Services;

internal interface IReturnService
{
    public Task<GetBookingResponse> GetBookingByIdAsync(Guid bookingId);
    public CheckInventoryNumberResponse CheckInventoryNumber(string inventoryNumber, Booking booking);
    public Task<RegisterReturnResponse> RegisterReturnAsync(Booking booking, string inventoryNumber, Guid staffId, bool isAvailable);
}
