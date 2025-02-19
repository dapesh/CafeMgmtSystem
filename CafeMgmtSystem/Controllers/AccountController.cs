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
        [Authorize]
        [HttpGet("auth")]
        public IActionResult Auth()
        {
            return Ok(true);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!model.PhoneNumber.StartsWith("+977"))
                {
                    return BadRequest("Phone number must start with +977.");
                }
                var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
                if (existingUser != null)
                {
                    return BadRequest(new { Code = "400", Message = "User with this Phone Number is already Registered " });
                }
                var user = new ApplicationUser { UserName = model.FullName.Replace(" ", ""), Email = model.Email, PhoneNumber = model.PhoneNumber };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(new { Code = "400", Message = result.Errors.Select(e => e.Description) });
                }
                await _userManager.AddToRoleAsync(user, "User");
                return Ok(new { Message = "User registered successfully", Code = "200" });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    Code = 500,
                    Message = "An unexpected error occurred. Please try again later.",
                    Detailed = ex.InnerException?.Message ?? ex.Message
                });
            }
            
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var user = await _userManager.Users
                                     .FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
                if (user == null)
                {
                    return Unauthorized(new { Code = "400", Message = "Invalid login attempt" });
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (!result.Succeeded)
                {
                    return Unauthorized(new { Code = "400", Message = "Invalid login attempt" });
                }

                var accessToken = await GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();
                var RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                return Ok(new TokenModel { Code = "200", Message = "Success", Token = accessToken, RefreshToken = refreshToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Code = 500,
                    Message = "An unexpected error occurred. Please try again later.",
                    Detailed = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<System.Security.Claims.Claim>
            {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName),
            new System.Security.Claims.Claim("FirstName", user.UserName),
            new System.Security.Claims.Claim("PhoneNumber", user.PhoneNumber),
            new System.Security.Claims.Claim("Email", user.Email)
            };
            foreach (var role in userRoles)
            {
                claims.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(5),
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
            var username = user.UserName;
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
            var RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(30);
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
                var emailResult = await _mailService.SendEmailAsync(new MailRequest { ToEmail = email });
                var otpResults = _mailService.GetOTPDetails(phoneNumber);

                return Ok(new Common
                {
                    Message = "OTP sent successfully.",
                    Type = "success",
                    Code = StatusCodes.Status200OK,
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
                return NotFound(new { Code="400", Message = "User not found" });
            }
            if(otpResults == null)
            {
                return BadRequest(new { Code = "400", Message = "Invalid OTP."});
            }
            if(otpResults.isVerified == "y" && request.ProcessId == otpResults.Id)
            {
                return BadRequest(new { Code = "400", Message = "OTP already verified" });
            }
            if (request.Otp != otpResults.Otp && request.ProcessId != otpResults.Id)
            {
                return BadRequest(new { Code = "400", Message = "Invalid OTP." });
            }
            if(request.ProcessId != otpResults.Id)
            {
                return BadRequest(new { Code = "400", Message = "Invalid Process ID" });
            }
            if (otpResults.CreateDate > otpResults.OtpExpiryTime)
            {
                return BadRequest(new { Code = "400", Message = "Otp Has Expired" });
            }
            otpResults.isVerified = "y";
            _mailService.UpdateOtpVerificationStatus(otpResults);
            return Ok(new Common
            {
                Message = "OTP verified successfully.",
                Type = "success",
                Code = StatusCodes.Status200OK,
                ProcessId = otpResults.Id
            });
        }
    }
}
