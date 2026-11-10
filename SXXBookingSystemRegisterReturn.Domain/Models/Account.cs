namespace SXXBookingSystemRegisterReturn.Domain.Models;

public abstract class Account
{
    public Guid AccountId { get; set; }
    public string Password { get; set; }
    bool IsActive { get; set; }

    public Account() { }

    public Account(Guid accountId, string password, bool isActive)
    {
        AccountId = accountId;
        Password = password;
        IsActive = isActive;
    }
}
