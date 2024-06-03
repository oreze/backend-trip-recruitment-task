using BackendTripRecruitmentTask.Application.Dtos.Trips;
using MediatR;

namespace BackendTripRecruitmentTask.Application.Commands;

public record CreateTripCommand(CreateTripDto CreateTripDto) : IRequest<int>;