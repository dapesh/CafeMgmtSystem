using CafeManagementSystem.Models;
using CafeMgmtSystem.Models;
using CafeMgmtSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace CafeMgmtSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly MailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        public AccountController(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,
                                  IConfiguration configuration,
                                  MailService mailService,
                                  ITokenService tokenService)
                                    {
                                        _userManager = userManager;
                                        _signInManager = signInManager;
                                        _configuration = configuration;
                                        _mailService = mailService;
                                        _tokenService = tokenService;
                                    }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!model.PhoneNumber.StartsWith("+977"))
            {
                return BadRequest("Phone number must start with +977.");
            }
            var user = new ApplicationUser { UserName = model.FirstName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName,PhoneNumber = model.PhoneNumber };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok(new { Message = "User registered successfully" , Code = "200"});
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid login attempt" });
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { Message = "Invalid login attempt" });
            }

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            //user.RefreshToken = refreshToken;
            var RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 
            //await _userManager.UpdateAsync(user);
            return Ok(new TokenModel { Code="200", Message="Success", Token = accessToken, RefreshToken = refreshToken });
        }
        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName),
            new System.Security.Claims.Claim("FirstName", user.FirstName),
            new System.Security.Claims.Claim("LastName", user.LastName),
            new System.Security.Claims.Claim("PhoneNumber", user.PhoneNumber),
            new System.Security.Claims.Claim("Email", user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [Authorize] 
        [HttpGet("profile")]
        public async Task<IActionResult> GetUsernameById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();  
            }
            var username = user.FirstName + " " + user.LastName;
            return Ok(username);
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            if (model is null)
            {
                return BadRequest("Invalid client request");
            }
            var claimsList = User.Claims.ToList();
            var customerID = string.Empty;
            int index = 0;
            if (index < claimsList.Count)
            {
                var claim = claimsList[index];
                customerID = claim.Value;
            }
            var user = await _userManager.FindByIdAsync(customerID);
            
            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            return Ok(new
            {
                Code = "200",
                Message = "Success",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp()
        {
            try
            {
                var phoneNumber = _tokenService.GetUserDetailsFromToken("PhoneNumber");
                _mailService.LogTable(phoneNumber);
                var email = _tokenService.GetUserDetailsFromToken("Email");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var emailResult = _mailService.SendEmailAsync(new MailRequest { ToEmail = email });
                var otpResults = _mailService.GetOTPDetails(phoneNumber);

                return Ok(new Common
                {
                    Message = "OTP sent successfully.",
                    Type = "success",
                    StatusCode = StatusCodes.Status200OK,
                    ProcessId = otpResults.Id,
                });
            }
            catch (Exception ex) 
            {
                _mailService.LogTable(ex.Message);
                return Unauthorized(ex.Message);
            }
        }
        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var phoneNumber = _tokenService.GetUserDetailsFromToken("PhoneNumber");
            var email = _tokenService.GetUserDetailsFromToken("Email");
            var user =  _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            var otpResults = new OtpHandler();
            if (user != null)
            {
                otpResults = _mailService.GetOTPDetails(phoneNumber);
            }
            else
            {
                return NotFound("User not found");
            }
            if (request.Otp != otpResults.Otp && request.ProcessId != otpResults.Id)
            {
                return BadRequest("Invalid OTP.");
            }
            if (otpResults.CreateDate > otpResults.OtpExpiryTime)
            {
                return Ok("Otp Has Expired");
            }
            otpResults.isVerified = "y";
            _mailService.UpdateOtpVerificationStatus(otpResults);
            return Ok(new Common
            {
                Message = "OTP verified successfully.",
                Type = "success",
                StatusCode = StatusCodes.Status200OK,
                ProcessId = otpResults.Id
            });
        }
    }
}
