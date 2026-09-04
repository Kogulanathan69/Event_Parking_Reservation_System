using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Event_Parking_Reservation_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (result == null)
            {
                return BadRequest(new
                {
                    message = "Email already exists."
                });
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (!result)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            return Ok(new
            {
                message = "OTP sent to your email"
            });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            var result = await _authService.VerifyOtpAsync(dto);

            if (result == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid or expired OTP."
                });
            }

            return Ok(result);
        
        }

        [HttpPost("verify-register-otp")]
        public async Task<IActionResult> VerifyRegisterOtp(VerifyOtpDto dto)
        {
            var result = await _authService.VerifyRegisterOtpAsync(dto);

            if (!result)
            {
                return BadRequest(new
                {
                    message = "Invalid or expired OTP."
                });
            }

            return Ok(new
            {
                message = "Email verified successfully. You can now login."
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (string.IsNullOrEmpty(userIdValue))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdValue);

            var result = await _authService.GetCurrentUserAsync(userId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var result = await _authService.ForgotPasswordAsync(dto);

            if (!result)
            {
                return BadRequest(new
                {
                    message = "Email not found."
                });
            }

            return Ok(new
            {
                message = "Password reset OTP sent to your email."
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);

            if (!result)
            {
                return BadRequest(new
                {
                    message = "Invalid OTP, expired OTP, or passwords do not match."
                });
            }

            return Ok(new
            {
                message = "Password reset successfully. Please login."
            });
        }

    }
}