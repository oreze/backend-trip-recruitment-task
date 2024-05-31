using BackendTripRecruitmentTask.Application.Dtos.Trips;

namespace BackendTripRecruitmentTask.Application.Services;

public interface ITripService
{
    public Task<int> CreateTrip(CreateTripDto createTripDto);
}