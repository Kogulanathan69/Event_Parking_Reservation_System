using Microsoft.Identity.Client;

namespace Event_Parking_Reservation_System.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int EventId { get; set; }

        public DateTime BookingDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public DateTime ExpiresAt { get; set; }

        public List<int> SeatIds { get; set; } = new List<int>();


    }
}
