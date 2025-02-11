using System.ComponentModel.DataAnnotations;

namespace CafeMgmtSystem.Models
{
    public class ForgotPasswordModel
    {
        //[Phone]
        //public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class VerifyOtpModel
    {
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Otp { get; set; }
    }

}
