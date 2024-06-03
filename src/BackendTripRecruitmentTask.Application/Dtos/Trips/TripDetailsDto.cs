namespace BackendTripRecruitmentTask.Application.Dtos.Trips;

public record TripDetailsDto(
    string Name,
    string Country,
    string? Description,
    DateTime StartDate,
    int NumberOfSeats,
    IEnumerable<RegistrationDetailsDto> RegistrationDetails);