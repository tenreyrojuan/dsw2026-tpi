using Microsoft.AspNetCore.Identity;

namespace Dsw2026Tpi.Data.Identity;

public sealed class ApplicationUser: IdentityUser
{
    public bool Deleted { get; private set; } = false;
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; private set; } = DateTime.Now;

    public ApplicationUser(string userName, string email)
        : base(userName) 
    {
        Email = email;
    }
    public void SetDelete()
    {
        Deleted = true;
        UpdatedAt = DateTime.Now;
    }
}
