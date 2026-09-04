using Event_Parking_Reservation_System.DTOs;

namespace Event_Parking_Reservation_System.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);

        Task<bool> VerifyRegisterOtpAsync(VerifyOtpDto dto);

        Task<bool> LoginAsync(LoginDto dto);

        Task<AuthResponseDto?> VerifyOtpAsync(VerifyOtpDto dto);

        Task<UserDto?> GetCurrentUserAsync(int userId);

        Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto);

        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    }
}