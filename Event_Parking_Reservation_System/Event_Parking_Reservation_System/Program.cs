using Event_Parking_Reservation_System.Data;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
=======
using Event_Parking_Reservation_System.Interfaces;
using Event_Parking_Reservation_System.Services;
>>>>>>> 03a8f1c (Complete booking module with expiry and seat validation)



namespace Event_Parking_Reservation_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

<<<<<<< HEAD
=======
            // Service registration for dependency injection

            builder.Services.AddScoped<IBookingService, BookingService>();

            builder.Services.AddHostedService<BookingExpirationService>();

            builder.Services.AddScoped<IPaymentService, PaymentService>();
>>>>>>> 03a8f1c (Complete booking module with expiry and seat validation)


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
