using CafeMgmtSystem.Models;
using System.Data;
using Dapper;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MailKit.Search;
using Org.BouncyCastle.Crypto;
using static System.Net.WebRequestMethods;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CafeMgmtSystem.Services
{
    public class MailService : IMailService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly MailSettings _mailSettings;
        private readonly ITokenService _tokenService;
        public MailService(IDbConnectionFactory dbConnectionFactory, IOptions<MailSettings> mailSettings, ITokenService tokenService)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _mailSettings = mailSettings.Value;
            _tokenService = tokenService;
        }
        private IDbConnection Connection => _dbConnectionFactory.CreateConnection();
        public OtpHandler GetOTPDetails(string phoneNumber)
        {
            using (var connection = Connection)
            {
                connection.Open();
                string query = @"Select * from OtpHandlers  
                                WHERE PhoneNumber = @PhoneNumber AND ISVERIFIED='p'";
                var result =  connection.Query<OtpHandler>(query, new { PhoneNumber = phoneNumber });
                return result.FirstOrDefault();
            }
        }

        public void LogTable(string Message)
        {
            using (var connection = Connection)
            {
                connection.Open();
                string query = @"INSERT INTO LogTable (LogMessage) VALUES (@Message)";
                connection.Execute(query, new { Message = Message });
            }
        }

        public async Task<Common> SendEmailAsync(MailRequest mailRequest)
        {
            using (var connection = Connection)
            {
                // Retrieve phone number and email from token
                var userPhone = _tokenService.GetUserDetailsFromToken("PhoneNumber");
                var userEmail = _tokenService.GetUserDetailsFromToken("Email");
                connection.Open();  // Open connection asynchronously

                // Ensure proper parameterized query to prevent SQL injection
                var query = @"
                    SELECT Email,otp 
                    FROM OtpHandlers 
                    WHERE PhoneNumber = @PhoneNumber
                    AND (isVerified IS NULL OR isVerified <> 'y')";

                var optDetails = await connection.QueryAsync<OtpHandler>(query, new { PhoneNumber = userPhone });
                if (!optDetails.Any())
                {
                    return new Common
                    {
                        Message = "Invalid OTP",
                        Type = "error",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                // Begin transaction
                using var transaction = connection.BeginTransaction();
                try
                {
                    // Invalidate previous OTPs
                    var updateRows = await connection.ExecuteAsync(
                        @"UPDATE OtpHandlers SET isVerified = 'n' WHERE PhoneNumber = @PhoneNumber",
                        new { PhoneNumber = userPhone },
                        transaction
                    );

                    // Generate new OTP
                    var otpCode = new Random().Next(100000, 999999).ToString();
                    var OtpExpiryTime = DateTime.UtcNow.AddMinutes(5);
                    var newOtp = new OtpHandler
                    {
                        Email = userEmail,
                        PhoneNumber = userPhone,
                        isVerified = "p",  // 'p' is for pending or unverified
                        CreateDate = DateTime.UtcNow,
                        Otp = otpCode,
                        OtpExpiryTime= OtpExpiryTime
                    };

                    // Insert new OTP record
                    var insertRows = await connection.ExecuteAsync(
                        "INSERT INTO OtpHandlers (Email, PhoneNumber, IsVerified, CreateDate, Otp, OtpExpiryTime) VALUES (@Email, @PhoneNumber, @isVerified, @CreateDate, @Otp, @OtpExpiryTime)",
                        newOtp,
                        transaction
                    );

                    // Commit the transaction
                     transaction.Commit();

                    // Send email asynchronously
                    mailRequest.Subject = "Forget Password";
                    mailRequest.Body = Function.Emailtemp(userEmail, otpCode);

                    var email = new MimeMessage();
                    email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                    email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                    email.Subject = mailRequest.Subject;

                    var builder = new BodyBuilder { HtmlBody = mailRequest.Body };
                    email.Body = builder.ToMessageBody();

                    using var smtp = new SmtpClient();
                    await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                    //await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                    //await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);

                    return new Common
                    {
                        Message = "OTP Sent Successfully",
                        Type = "success",
                        StatusCode = StatusCodes.Status200OK,
                        Email = userEmail
                    };
                }
                catch (Exception ex)
                {
                    // Log the exception if needed (e.g., using a logging framework)
                    Console.WriteLine($"Error sending OTP: {ex.Message}");

                    // Rollback the transaction if something goes wrong
                     transaction.Rollback();

                    return new Common
                    {
                        Message = "Failed to send OTP",
                        Type = "error",
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
            }
        }
        public Task UpdateOtpAsync(string userId, string otp, DateTime otpExpiryTime, bool isVerified)
        {
            using (var connection = Connection)
            {
                isVerified = true;
                connection.Open();
                string query = @"
                                UPDATE OtpHandlers  
                                SET Otp = @Otp,IsVerified = @IsVerified, OtpExpiryTime = @OtpExpiryTime
                                WHERE Id = @Id";
                var result =  connection.ExecuteAsync(query, new { Otp = otp, OtpExpiryTime = otpExpiryTime, UserId = userId });
                return result;
            }
        }

        public void UpdateOtpVerificationStatus(OtpHandler otpResults)
        {
            using (var connection = Connection)
            {
                connection.Open();
                string query = @"
                        UPDATE OtpHandlers
                        SET IsVerified = 'y'
                        WHERE Id = @Id";
                 connection.Execute(query, new { Id = otpResults.Id });
            }
        }
    }
}
