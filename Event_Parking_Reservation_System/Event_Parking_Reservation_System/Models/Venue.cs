namespace Event_Parking_Reservation_System.Models
{
    public class Venue
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public List<Event> Events { get; set; } = new List<Event>();
    }
}