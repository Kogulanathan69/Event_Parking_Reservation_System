namespace Event_Parking_Reservation_System.DTOs
{
    public class PaymentDto
    {

        public int Id { get; set; }

        public int BookingId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; }

        public string TransactionReference { get; set; } = string.Empty;
    }
}
