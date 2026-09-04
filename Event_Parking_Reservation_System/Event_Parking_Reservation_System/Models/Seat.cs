namespace Event_Parking_Reservation_System.Models
{
    public class Seat
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string Row { get; set; } = string.Empty;

        public int Number { get; set; }

        public string SeatType { get; set; } = "Standard";

        public decimal Price { get; set; }

        public Event? Event { get; set; }

        public ICollection<BookingSeat> BookingSeats { get; set; }
            = new List<BookingSeat>();
    }
}