using Microsoft.AspNetCore.Identity;

namespace CafeManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public override DateTimeOffset? LockoutEnd { get; set; }
    }
}
