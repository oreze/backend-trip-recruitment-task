using MediatR;

namespace BackendTripRecruitmentTask.Application.Commands;

public record RegisterForTripCommand(int ID, string Email) : IRequest;