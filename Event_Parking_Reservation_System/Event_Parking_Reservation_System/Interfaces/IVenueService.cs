using Event_Parking_Reservation_System.DTOs;

namespace Event_Parking_Reservation_System.Interfaces
{
    public interface IVenueService
    {
        Task<List<VenueDto>> GetAllVenuesAsync();

        Task<VenueDto?> GetVenueByIdAsync(int id);

        Task<VenueDto> CreateVenueAsync(CreateVenueDto dto);

        Task<bool> UpdateVenueAsync(int id, UpdateVenueDto dto);

        Task<bool> DeleteVenueAsync(int id);

        Task<bool> IsVenueAvailableAsync(
          int venueId,
            DateTime startDateTime,
            DateTime endDateTime
        );
    }
}