using BackendTripRecruitmentTask.Application.Dtos.Trips;
using BackendTripRecruitmentTask.Application.Queries;
using BackendTripRecruitmentTask.Application.Services;
using MediatR;

namespace BackendTripRecruitmentTask.Application.Handlers;

public class GetSingleTripQueryHandler(ITripService tripService)
    : IRequestHandler<GetSingleTripQuery, TripDetailsDto>
{
    private readonly ITripService _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));

    public async Task<TripDetailsDto> Handle(GetSingleTripQuery request,
        CancellationToken cancellationToken)
    {
        return await _tripService.GetDetails(request.ID);
    }
}