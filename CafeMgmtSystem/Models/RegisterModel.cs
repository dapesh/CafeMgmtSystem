using System.ComponentModel.DataAnnotations;

namespace CafeManagementSystem.Models
{
    public class RegisterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [Phone]
        [Required]
        //[RegularExpression(@"^\+977\d{9}$", ErrorMessage = "Invalid phone number format. Please enter a valid Nepali phone number starting with +977.")]
        public string PhoneNumber { get; set; }
    }
}
