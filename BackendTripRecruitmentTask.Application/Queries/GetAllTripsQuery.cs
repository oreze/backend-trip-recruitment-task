using BackendTripRecruitmentTask.Application.Dtos.Trips;
using MediatR;

namespace BackendTripRecruitmentTask.Application.Queries;

public record GetAllTripsQuery: IRequest<IEnumerable<TripListDto>>;