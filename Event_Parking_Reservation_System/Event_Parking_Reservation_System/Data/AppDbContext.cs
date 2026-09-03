using Event_Parking_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Parking_Reservation_System.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }


        public DbSet<BookingSeat> BookingSeats { get; set; }


        public DbSet<Payment> Payments { get; set; }

        public DbSet<PrivateEventBooking> PrivateEventBookings { get; set; }
    }
}