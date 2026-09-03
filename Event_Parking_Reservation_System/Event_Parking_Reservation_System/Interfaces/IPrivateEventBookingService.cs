using Event_Parking_Reservation_System.DTOs;
using Event_Parking_Reservation_System.Models;

namespace Event_Parking_Reservation_System.Interfaces
{
    public interface IPrivateEventBookingService
    {
        Task<List<PrivateEventBooking>> GetAllAsync();

        Task<PrivateEventBooking?> GetByIdAsync(int id);

        Task<PrivateEventBooking> CreateAsync(CreatePrivateEventBookingDto dto);


        Task<List<PrivateEventBooking>> GetByUserIdAsync(int userId);


        Task<bool> CancelAsync(int id);

        Task<bool> ConfirmAsync(int id);
    }
}