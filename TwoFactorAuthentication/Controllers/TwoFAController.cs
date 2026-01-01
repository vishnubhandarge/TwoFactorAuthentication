using Microsoft.AspNetCore.Mvc;
using OtpNet;
using TwoFactorAuthentication.Data;
using TwoFactorAuthentication.DTOs;
using TwoFactorAuthentication.Models;

namespace TwoFactorAuthentication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TwoFAController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public TwoFAController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUp signUpDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingUser = _dbContext.Users.FirstOrDefault(u => u.Username == signUpDto.Username);

            if (existingUser != null)
            {
                return Conflict("Username already exists.");
            }

            var bytes = KeyGeneration.GenerateRandomKey(20);

            var TwoFactorSecret = Base32Encoding.ToString(bytes);

            ApplicationUser user = new ApplicationUser()
            {
                FullName = signUpDto.FullName,
                Username = signUpDto.Username,
                Password = signUpDto.Password,
                TwoFactorEnabled = true,
                TwoFactorSecret = TwoFactorSecret
            };

            var result = await _dbContext.Users.AddAsync(user);
            if (result != null)
            {
                await _dbContext.SaveChangesAsync();
                return Ok("User registered successfully.");
            }
            return StatusCode(500, "An error occurred while registering the user.");
        }

        [HttpPost]
        public IActionResult SignIn([FromBody] SignInDTO signInDto)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == signInDto.Username && u.Password == signInDto.Password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            if (user.TwoFactorEnabled)
            {
                return Ok(new { Message = "Enter 2FA code" });
            }
            return Ok("Sign-in successful without 2FA.");
        }

        [HttpPost]
        public IActionResult VerifyTwoFactor([FromBody] VerifyDTO verifyDto)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == verifyDto.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username.");
            }
            var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorSecret));
            bool isValid = totp.VerifyTotp(verifyDto.TwoFactorCode, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
            if (isValid)
            {
                return Ok("2FA verification successful. Sign-in complete.");
            }
            else
            {
                return Unauthorized("Invalid 2FA code.");
            }
        }
    }
}
