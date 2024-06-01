using BackendTripRecruitmentTask.Application.Dtos.Trips;

namespace BackendTripRecruitmentTask.Application.Services;

public interface ITripService
{
    public Task<int> CreateTrip(CreateTripDto createTripDto);
    public Task EditTrip(int id, EditTripDto editTripDto);
    public Task<bool> DeleteTrip(int id);
    public Task<IEnumerable<TripListDto>> GetAll();
    public Task<IEnumerable<TripSearchDto>> GetByCountry(string country);
    public Task<TripDetailsDto> GetDetails(int id);
    public Task Register(int tripID, string email);
}