namespace Event_Parking_Reservation_System.DTOs
{
    public class ParkingReservationDto
    {
        public int Id { get; set; }

        public int BookingId { get; set; }

        public int ParkingAreaId { get; set; }

        public int? ParkingSlotId { get; set; }

        public string VehicleNumber { get; set; } = string.Empty;

        public decimal ParkingFee { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? QrToken { get; set; }

        public DateTime ReservationDate { get; set; }
    }
}
