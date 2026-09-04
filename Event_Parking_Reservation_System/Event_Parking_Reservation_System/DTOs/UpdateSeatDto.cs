namespace Event_Parking_Reservation_System.DTOs
{
    public class UpdateSeatDto
    {
        public string Row { get; set; } = string.Empty;

        public int Number { get; set; }

        public string SeatType { get; set; } = "Standard";

        public decimal Price { get; set; }
    }
}
