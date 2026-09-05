namespace Event_Parking_Reservation_System.Models
{
    public class ParkingArea
    {
        public int Id { get; set; }

        // Foreign Key to Venue
        public int VenueId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int TotalSlots { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Venue? Venue { get; set; }

        public ICollection<ParkingSlot> ParkingSlots { get; set; } = new List<ParkingSlot>();

        public ICollection<ParkingReservation> ParkingReservations { get; set; } = new List<ParkingReservation>();
    }
}
