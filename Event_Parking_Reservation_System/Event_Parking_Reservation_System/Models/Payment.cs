namespace Event_Parking_Reservation_System.Models
{
    public class Payment
    {
        public int Id { get; set; }

        // Foreign Key to Booking (One Booking can have multiple Payment attempts)
        public int BookingId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty; // Card, Cash, Bank Transfer

        public string Status { get; set; } = "Pending"; // Pending, Success, Failed

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public string TransactionReference { get; set; } = string.Empty; // Simulated transaction reference

        // Navigation Property
        public Booking? Booking { get; set; }
    }
}
