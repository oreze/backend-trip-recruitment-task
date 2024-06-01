using BackendTripRecruitmentTask.Application.Dtos.Trips;

namespace BackendTripRecruitmentTask.Application.Services;

public interface ITripService
{
    public Task<int> CreateTrip(CreateTripDto createTripDto);
    public Task<bool> DeleteTrip(int id);
    public Task<IEnumerable<TripListDto>> GetAll();
    public Task<IEnumerable<TripSearchDto>> GetByCountry(string country);
}