namespace BackendTripRecruitmentTask.Application.Dtos.Trips;

public record CreateTripDto(
    string Name,
    string? Description,
    DateTime StartDate,
    int NumberOfSeats,
    string CountryName);