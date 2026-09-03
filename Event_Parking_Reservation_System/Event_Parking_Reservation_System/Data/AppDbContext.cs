
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

        public DbSet<User> Users { get; set; }
        
        public DbSet<Role> Roles { get; set; }
    }
}