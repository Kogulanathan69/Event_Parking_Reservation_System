using Event_Parking_Reservation_System.DTOs;

namespace Event_Parking_Reservation_System.Interfaces
{
    public interface IPaymentService
    {
        Task<List<PaymentDto>> GetAllPaymentsAsync();

        Task<PaymentDto?> GetPaymentByIdAsync(int id);

        Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto);


        Task<PaymentDto?> GetPaymentByBookingIdAsync(int bookingId);
    }
}