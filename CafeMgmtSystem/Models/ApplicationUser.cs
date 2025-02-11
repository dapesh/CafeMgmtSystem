using Microsoft.AspNetCore.Identity;

namespace CafeManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override DateTimeOffset? LockoutEnd { get; set; }
        public string? Otp { get; set; }  // Store hashed OTP
        public string? PhoneNumber { get; set; }  // Store hashed OTP
        public string? Email { get; set; }  
        public DateTimeOffset? OtpExpiryTime { get; set; }
    }
}
