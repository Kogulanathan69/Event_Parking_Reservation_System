namespace Event_Parking_Reservation_System.DTOs
{
    public class CreatePaymentDto
    {
        public int BookingId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;
    }
}
