using BackendTripRecruitmentTask.Application.Dtos.Trips;
using MediatR;

namespace BackendTripRecruitmentTask.Application.Queries;

public record GetSingleTripQuery(int ID) : IRequest<TripDetailsDto>;