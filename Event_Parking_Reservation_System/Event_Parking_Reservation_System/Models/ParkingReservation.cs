namespace Event_Parking_Reservation_System.Models
{
    public class ParkingReservation
    {
        public int Id { get; set; }

        // Foreign Key to Booking
        public int BookingId { get; set; }

        // Foreign Key to ParkingArea
        public int ParkingAreaId { get; set; }

        // Foreign Key to ParkingSlot (Nullable for private/event-level QR reservations)
        public int? ParkingSlotId { get; set; }

        public string VehicleNumber { get; set; } = string.Empty;

        public decimal ParkingFee { get; set; }

        public string Status { get; set; } = "Pending";

        // Opaque random token used to generate QR code URLs safely (unique when not null)
        public string? QrToken { get; set; }

        public DateTime ReservationDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Booking? Booking { get; set; }

        public ParkingArea? ParkingArea { get; set; }

        public ParkingSlot? ParkingSlot { get; set; }
    }
}
