using System.Security.Principal;

namespace SXXBookingSystemRegisterReturn.Domain.Models;

public class StaffAccount : Account
{
    public string UserName { get; set; }

    public StaffAccount() { }

    public StaffAccount(Guid accountId, string password, bool isActive, string userName)
        : base(accountId, password, isActive)
    {
        UserName = userName;
    }
}
