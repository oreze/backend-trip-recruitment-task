namespace BackendTripRecruitmentTask.Domain.Exceptions;

public class InputException(string? paramName, string? message) : ArgumentException(message, paramName);