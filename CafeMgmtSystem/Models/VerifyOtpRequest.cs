using System.ComponentModel.DataAnnotations;

namespace CafeMgmtSystem.Models
{
    public class VerifyOtpRequest
    {
        [Required]
        public Guid ? ProcessId { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits.")]
        public string Otp { get; set; }
    }
}
