

using Event_Parking_Reservation_System.Models;
//03a8f1c (Complete booking module with expiry and seat validation)
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data;

namespace Event_Parking_Reservation_System.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

      // HEAD
        


        public DbSet<Booking> Bookings { get; set; }

        public DbSet<BookingSeat> BookingSeats { get; set; }

        public DbSet<Payment> Payments { get; set; }
        03a8f1c (Complete booking module with expiry and seat validation)
    }
}