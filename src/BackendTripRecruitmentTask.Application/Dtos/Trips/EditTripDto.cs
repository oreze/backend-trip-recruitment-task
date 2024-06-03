namespace BackendTripRecruitmentTask.Application.Dtos.Trips;

public record EditTripDto(
    string? Name,
    string? Description,
    DateTime? StartDate,
    int? NumberOfSeats,
    string? CountryName);