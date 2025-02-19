using CafeMgmtSystem.Models;

namespace CafeMgmtSystem.Services
{
    public interface IMailService
    {
        Task<Common> SendEmailAsync(MailRequest mailRequest);
        Task UpdateOtpAsync(string userId, string otp, DateTime otpExpiryTime, bool isVerified);
        OtpHandler GetOTPDetails(string Email);
        void UpdateOtpVerificationStatus(OtpHandler otpResults);
        void LogTable(string Message);
    }
}
