using Microsoft.AspNetCore.Identity;

namespace CafeManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override DateTimeOffset? LockoutEnd { get; set; }
    }
}
