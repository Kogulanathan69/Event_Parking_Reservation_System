namespace Event_Parking_Reservation_System.DTOs
{
    public class CreateVenueDto
    {
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public int Capacity { get; set; }
    }
}