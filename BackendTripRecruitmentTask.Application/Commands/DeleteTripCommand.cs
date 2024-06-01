using MediatR;

namespace BackendTripRecruitmentTask.Application.Commands;

public record DeleteTripCommand(int ID) : IRequest<bool>;