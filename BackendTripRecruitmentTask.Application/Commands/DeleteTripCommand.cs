using MediatR;

namespace BackendTripRecruitmentTask.Application.Commands;

public record DeleteTripCommand(string Name) : IRequest<bool>;