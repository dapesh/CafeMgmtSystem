using System.ComponentModel.DataAnnotations;

namespace CafeManagementSystem.Models
{
    public class LoginModel
    {
        [Phone]
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
