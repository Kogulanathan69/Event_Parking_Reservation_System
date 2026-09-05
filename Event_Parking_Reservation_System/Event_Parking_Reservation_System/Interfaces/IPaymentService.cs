using Event_Parking_Reservation_System.DTOs;

namespace Event_Parking_Reservation_System.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto);

        Task<PaymentDto?> GetPaymentByIdAsync(int id);

        Task<PaymentDto?> GetPaymentByBookingIdAsync(int bookingId);

        Task<List<PaymentDto>> GetPaymentsByBookingIdAsync(int bookingId);

        Task<List<PaymentDto>> GetAllPaymentsAsync();

        Task<bool> HasSuccessfulPaymentAsync(int bookingId);
    }
}