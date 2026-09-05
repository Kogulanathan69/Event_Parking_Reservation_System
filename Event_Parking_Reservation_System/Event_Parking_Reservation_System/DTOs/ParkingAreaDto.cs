namespace Event_Parking_Reservation_System.DTOs
{
    public class ParkingAreaDto
    {
        public int Id { get; set; }

        public int VenueId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int TotalSlots { get; set; }

        public bool IsActive { get; set; }
    }
}
