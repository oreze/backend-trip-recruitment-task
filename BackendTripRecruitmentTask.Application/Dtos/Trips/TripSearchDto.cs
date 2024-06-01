namespace BackendTripRecruitmentTask.Application.Dtos.Trips;

public record TripSearchDto(
    string Name,
    string Country,
    DateTime StartDate);