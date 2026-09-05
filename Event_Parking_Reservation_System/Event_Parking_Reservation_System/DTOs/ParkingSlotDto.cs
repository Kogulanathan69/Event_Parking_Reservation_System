namespace Event_Parking_Reservation_System.DTOs
{
    public class ParkingSlotDto
    {
        public int Id { get; set; }

        public int ParkingAreaId { get; set; }

        public string SlotNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
