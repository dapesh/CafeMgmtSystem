using System.ComponentModel.DataAnnotations;

namespace CafeMgmtSystem.Models
{
    public class OtpHandler
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; }
        public string Otp { get; set; }
        public DateTime OtpExpiryTime { get; set; }
        public string PhoneNumber { get; set; }
        public string isVerified { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
