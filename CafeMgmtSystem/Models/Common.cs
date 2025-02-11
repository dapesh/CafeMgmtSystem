namespace CafeMgmtSystem.Models
{
    public class Common
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public int StatusCode { get; set; }
        public string Email { get; set; }
        public string Otp { get; set; }
        public Guid ProcessId { get; set; }
    }
}
