using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Event_Parking_Reservation_System.Data;
using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Interfaces;
using Event_Parking_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Parking_Reservation_System.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(
            AppDbContext context,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        // =====================================================
        // 1. REGISTER
        // =====================================================
        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
            {
                return null;
            }

            var customerRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Customer");

            if (customerRole == null)
            {
                customerRole = new Role
                {
                    Name = "Customer"
                };

                _context.Roles.Add(customerRole);
                await _context.SaveChangesAsync();
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = customerRole.Id,
                IsEmailVerified = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Register OTP
            string otp = Random.Shared
                .Next(100000, 1000000)
                .ToString();

            var registerOtp = new LoginOtp
            {
                UserId = user.Id,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                Purpose = "Register"
            };

            _context.LoginOtps.Add(registerOtp);
            await _context.SaveChangesAsync();

            await _emailService.SendOtpEmailAsync(
                user.Email,
                otp
            );

            return new AuthResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = customerRole.Name,

                // Register OTP verify ஆகும் வரை JWT வேண்டாம்
                Token = ""
            };
        }


        // =====================================================
        // 2. VERIFY REGISTER OTP
        // =====================================================
        public async Task<bool> VerifyRegisterOtpAsync(VerifyOtpDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return false;
            }

            var otpRecord = await _context.LoginOtps
                .Where(o =>
                    o.UserId == user.Id &&
                    o.OtpCode == dto.Otp &&
                    o.Purpose == "Register" &&
                    !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpRecord == null)
            {
                return false;
            }

            if (otpRecord.ExpiresAt < DateTime.UtcNow)
            {
                return false;
            }

            otpRecord.IsUsed = true;

            user.IsEmailVerified = true;

            await _context.SaveChangesAsync();

            return true;
        }


        // =====================================================
        // 3. LOGIN
        // =====================================================
        public async Task<bool> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return false;
            }

            if (!user.IsEmailVerified)
            {
                return false;
            }

            bool passwordCorrect =
                BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    user.PasswordHash
                );

            if (!passwordCorrect)
            {
                return false;
            }

            // Previous unused OTPs disable
            var oldOtps = await _context.LoginOtps
                .Where(o =>
                    o.UserId == user.Id &&
                    !o.IsUsed)
                .ToListAsync();

            foreach (var oldOtp in oldOtps)
            {
                oldOtp.IsUsed = true;
            }

            // New login OTP
            string otp = Random.Shared
                .Next(100000, 1000000)
                .ToString();

            var loginOtp = new LoginOtp
            {
                UserId = user.Id,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                Purpose = "Login"
            };

            _context.LoginOtps.Add(loginOtp);

            await _context.SaveChangesAsync();

            await _emailService.SendOtpEmailAsync(
                user.Email,
                otp
            );

            return true;
        }


        // =====================================================
        // 4. VERIFY LOGIN OTP
        // =====================================================
        public async Task<AuthResponseDto?> VerifyOtpAsync(
            VerifyOtpDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return null;
            }

            var otpRecord = await _context.LoginOtps
                .Where(o =>
                    o.UserId == user.Id &&
                    o.OtpCode == dto.Otp &&
                    o.Purpose == "Register" &&
                    !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpRecord == null)
            {
                return null;
            }

            if (otpRecord.ExpiresAt < DateTime.UtcNow)
            {
                return null;
            }

            otpRecord.IsUsed = true;

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role?.Name ?? "",

                Token = GenerateToken(
                    user,
                    user.Role?.Name ?? ""
                )
            };
        }


        // =====================================================
        // 5. GET CURRENT LOGIN USER
        // =====================================================
        public async Task<UserDto?> GetCurrentUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role?.Name ?? ""
            };
        }

        // =====================================================
        // 6. FotgotPasswordLogic
        // =====================================================

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return false;
            }

            // Old reset OTPs invalidate
            var oldOtps = await _context.LoginOtps
                .Where(o =>
                    o.UserId == user.Id &&
                    o.Purpose == "ResetPassword" &&
                    !o.IsUsed)
                .ToListAsync();

            foreach (var oldOtp in oldOtps)
            {
                oldOtp.IsUsed = true;
            }

            string otp = Random.Shared
                .Next(100000, 1000000)
                .ToString();

            var resetOtp = new LoginOtp
            {
                UserId = user.Id,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                Purpose = "ResetPassword"
            };

            _context.LoginOtps.Add(resetOtp);

            await _context.SaveChangesAsync();

            await _emailService.SendOtpEmailAsync(
                user.Email,
                otp
            );

            return true;
        }

        // =====================================================
        // 7. ResetPasswordLogic
        // =====================================================

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return false;
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return false;
            }

            var otpRecord = await _context.LoginOtps
                .Where(o =>
                    o.UserId == user.Id &&
                    o.OtpCode == dto.Otp &&
                    o.Purpose == "ResetPassword" &&
                    !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpRecord == null)
            {
                return false;
            }

            if (otpRecord.ExpiresAt < DateTime.UtcNow)
            {
                return false;
            }

            otpRecord.IsUsed = true;

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _context.SaveChangesAsync();

            return true;
        }


        // =====================================================
        // 8. GENERATE JWT TOKEN
        // =====================================================
        private string GenerateToken(
            User user,
            string roleName)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    user.Name
                ),

                new Claim(
                    ClaimTypes.Email,
                    user.Email
                ),

                new Claim(
                    ClaimTypes.Role,
                    roleName
                )
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}