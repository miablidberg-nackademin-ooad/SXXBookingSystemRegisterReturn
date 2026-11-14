namespace SXXBookingSystemRegisterReturn.Application.Dtos
{
    public record CheckInventoryNumberResponse
    (
        bool Success,
        string? ErrorMessage
    );
}