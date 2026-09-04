using System.ComponentModel.DataAnnotations;
namespace Event_Parking_Reservation_System.Models
{
    public class LoginOtp
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string OtpCode { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Purpose { get; set; } = string.Empty;

        public User? User { get; set; }

    }
}
