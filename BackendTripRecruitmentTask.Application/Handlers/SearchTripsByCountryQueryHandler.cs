using BackendTripRecruitmentTask.Application.Dtos.Trips;
using BackendTripRecruitmentTask.Application.Queries;
using BackendTripRecruitmentTask.Application.Services;
using MediatR;

namespace BackendTripRecruitmentTask.Application.Handlers;

public class SearchTripsByCountryQueryHandler(ITripService tripService)
    : IRequestHandler<SearchTripsByCountryQuery, IEnumerable<TripSearchDto>>
{
    private readonly ITripService _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));

    public async Task<IEnumerable<TripSearchDto>> Handle(SearchTripsByCountryQuery request,
        CancellationToken cancellationToken)
    {
        return await _tripService.GetByCountry(request.Country);
    }
}