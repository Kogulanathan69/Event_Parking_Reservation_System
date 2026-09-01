using Event_Parking_Reservation_System.DTOs;

namespace Event_Parking_Reservation_System.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingDto>> GetAllBookingsAsync();

        Task<BookingDto?> GetBookingByIdAsync(int id);

        Task<List<BookingDto>> GetBookingsByUserIdAsync(int userId);

        Task<BookingDto> CreateBookingAsync(CreateBookingDto dto);

        Task<bool> CancelBookingAsync(int id);

        Task<bool> ConfirmBookingAsync(int id);

        Task<int> ExpirePendingBookingsAsync();
    }
}
