namespace SXXBookingSystemRegisterReturn.Application.Dtos
{
    public record RegisterReturnResponse
    (
        bool Success,
        string? ErrorMessage
    );
}