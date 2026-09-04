using Event_Parking_Reservation_System.DTOs;

namespace Event_Parking_Reservation_System.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);

    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
}
