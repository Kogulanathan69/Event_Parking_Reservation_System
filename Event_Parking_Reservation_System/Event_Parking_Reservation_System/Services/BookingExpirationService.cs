using Event_Parking_Reservation_System.Interfaces;

namespace Event_Parking_Reservation_System.Services
{
    public class BookingExpirationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BookingExpirationService> _logger;

        public BookingExpirationService(
            IServiceScopeFactory scopeFactory,
            ILogger<BookingExpirationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var bookingService = scope.ServiceProvider
                    .GetRequiredService<IBookingService>();

                var expiredCount =
                    await bookingService.ExpirePendingBookingsAsync();

                if (expiredCount > 0)
                {
                    _logger.LogInformation(
                        "{Count} booking(s) expired automatically",
                        expiredCount
                    );
                }

                await Task.Delay(
                    TimeSpan.FromMinutes(1),
                    stoppingToken
                );
            }
        }
    }
}