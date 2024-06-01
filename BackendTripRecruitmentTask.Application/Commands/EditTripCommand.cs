using BackendTripRecruitmentTask.Application.Dtos.Trips;
using MediatR;

namespace BackendTripRecruitmentTask.Application.Commands;

public record EditTripCommand(int ID, EditTripDto EditTripDto): IRequest;
