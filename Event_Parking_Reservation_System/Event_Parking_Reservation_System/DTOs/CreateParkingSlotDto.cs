namespace Event_Parking_Reservation_System.DTOs
{
    public class CreateParkingSlotDto
    {
        public int ParkingAreaId { get; set; }

        public string SlotNumber { get; set; } = string.Empty;
    }
}
