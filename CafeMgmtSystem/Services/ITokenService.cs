using CafeManagementSystem.Models;

namespace CafeMgmtSystem.Services
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user);
        string GetUserDetailsFromToken(string Key);
    }
}
