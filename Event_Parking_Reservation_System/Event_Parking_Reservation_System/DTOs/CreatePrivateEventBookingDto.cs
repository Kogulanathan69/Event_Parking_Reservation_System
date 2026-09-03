namespace Event_Parking_Reservation_System.DTOs
{
    public class CreatePrivateEventBookingDto
    {
        public int UserId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public string EventName { get; set; } = string.Empty;

        public int VenueId { get; set; }

        public DateTime EventDate { get; set; }

        public int GuestCount { get; set; }

        public bool NeedParking { get; set; }

        public int? ParkingAreaId { get; set; }

        public decimal TotalAmount { get; set; }
    }
}