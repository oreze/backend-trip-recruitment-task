using BackendTripRecruitmentTask.Application.Dtos.Trips;
using MediatR;

namespace BackendTripRecruitmentTask.Application.Queries;

public record SearchTripsByCountryQuery(string Country) : IRequest<IEnumerable<TripSearchDto>>;