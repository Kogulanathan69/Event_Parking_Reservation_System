using System.ComponentModel.DataAnnotations;

namespace Event_Parking_Reservation_System.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

    }
}
