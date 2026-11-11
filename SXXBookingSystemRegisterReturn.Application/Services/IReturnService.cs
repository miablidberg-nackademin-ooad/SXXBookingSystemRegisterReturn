using SXXBookingSystemRegisterReturn.Application.Dtos;
using SXXBookingSystemRegisterReturn.Domain.Models;

namespace SXXBookingSystemRegisterReturn.Application.Services;

internal interface IReturnService
{
    public Task<GetBookingResponse> GetBookingByIdAsync(Guid bookingId);
    public Task<CheckInventoryNumberResponse> CheckInventoryNumberAsync(string inventoryNumber, Booking booking, Guid staffId);
    public Task<RegisterReturnResponse> RegisterReturnAsync(Booking booking, string inventoryNumber);
}
