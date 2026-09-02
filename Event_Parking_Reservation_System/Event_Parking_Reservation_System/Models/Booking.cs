using Microsoft.Identity.Client;

namespace Event_Parking_Reservation_System.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int EventId { get; set; }

        public DateTime BookingDate { get; set; }

        public string Status { get; set; } = "pending";

        public decimal TotalAmount { get; set; }

        public DateTime ExpiresAt { get; set; }


        public List<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    }
}
