namespace Event_Parking_Reservation_System.DTOs
{
    public class CreateBookingDto
    {


        public int UserId { get; set; }

        public int EventId { get; set; }

        public List<int> SeatIds { get; set; } = new();

        public decimal TotalAmount { get; set; } 
    }
}
