namespace Event_Parking_Reservation_System.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int BookingId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public string TransactionReference { get; set; } = string.Empty;

        public Booking? Booking { get; set; }
    }
}
