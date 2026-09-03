using Event_Parking_Reservation_System.Data;
using Event_Parking_Reservation_System.Interfaces;
using Event_Parking_Reservation_System.Services;
using Microsoft.EntityFrameworkCore;

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

            // Dependency Injection
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddHostedService<BookingExpirationService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();


            builder.Services.AddScoped<
                IPrivateEventBookingService,
                PrivateEventBookingService
            >();


            // CORS - Allow Angular frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy =>
                    {
                        policy
                            .WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            
            // IMPORTANT: CORS middleware
            app.UseCors("AllowAngular");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}