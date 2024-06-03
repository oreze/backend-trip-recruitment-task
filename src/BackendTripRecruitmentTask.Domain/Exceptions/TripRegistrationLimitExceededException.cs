namespace BackendTripRecruitmentTask.Domain.Exceptions;

public class TripRegistrationLimitExceededException(string? message) : Exception(message);