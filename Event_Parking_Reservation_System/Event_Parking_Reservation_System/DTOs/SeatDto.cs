namespace Event_Parking_Reservation_System.DTOs
{
    public class SeatDto
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string Row { get; set; } = string.Empty;

        public int Number { get; set; }

        public string SeatType { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}
