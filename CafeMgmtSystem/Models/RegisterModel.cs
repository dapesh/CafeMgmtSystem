using System.ComponentModel.DataAnnotations;

namespace CafeManagementSystem.Models
{
    public class RegisterModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Phone]
        [Required]
        //[RegularExpression(@"^\+977\d{9}$", ErrorMessage = "Invalid phone number format. Please enter a valid Nepali phone number starting with +977.")]
        public string PhoneNumber { get; set; }
    }
}
