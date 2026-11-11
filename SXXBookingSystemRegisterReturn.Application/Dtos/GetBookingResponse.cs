using SXXBookingSystemRegisterReturn.Domain.Models;

namespace SXXBookingSystemRegisterReturn.Application.Dtos
{
    public record GetBookingResponse
    (
        bool Success,
        Booking? Booking,
        string? ErrorMessage
    );
}