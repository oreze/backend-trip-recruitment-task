namespace BackendTripRecruitmentTask.Application.Dtos.Trips;

public record TripListDto(
    string Name,
    string Country,
    DateTime StartDate);