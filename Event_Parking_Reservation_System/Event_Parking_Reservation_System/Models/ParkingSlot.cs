namespace Event_Parking_Reservation_System.Models
{
    public class ParkingSlot
    {
        public int Id { get; set; }

        public int ParkingAreaId { get; set; }

        public string SlotNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ParkingArea? ParkingArea { get; set; }

        public ICollection<ParkingReservation> ParkingReservations { get; set; } = new List<ParkingReservation>();
    }
}
