namespace CafeManagementSystem.Models
{
    public class TokenModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
