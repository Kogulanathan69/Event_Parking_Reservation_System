using System.ComponentModel.DataAnnotations;

namespace Event_Parking_Reservation_System.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
