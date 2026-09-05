namespace Event_Parking_Reservation_System.DTOs
{
    public class CreateParkingReservationDto
    {
        public int BookingId { get; set; }

        public int ParkingAreaId { get; set; }

        public int? ParkingSlotId { get; set; }

        public string? VehicleNumber { get; set; }
    }
}
