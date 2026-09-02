namespace Event_Parking_Reservation_System.Models
{
    public class Event
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int VenueId { get; set; }

        public int CategoryId { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public decimal TicketPrice { get; set; }

        public decimal ParkingFee { get; set; }

        public int Capacity { get; set; }

        public Venue? Venue { get; set; }

        public EventCategory? Category { get; set; }
    }
}