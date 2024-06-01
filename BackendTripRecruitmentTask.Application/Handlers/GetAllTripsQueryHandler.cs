using BackendTripRecruitmentTask.Application.Dtos.Trips;
using BackendTripRecruitmentTask.Application.Queries;
using BackendTripRecruitmentTask.Application.Services;
using MediatR;

namespace BackendTripRecruitmentTask.Application.Handlers;

public class GetAllTripsQueryHandler(ITripService tripService): IRequestHandler<GetAllTripsQuery, IEnumerable<TripListDto>>
{
    private readonly ITripService _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));

    public async Task<IEnumerable<TripListDto>> Handle(GetAllTripsQuery request, CancellationToken cancellationToken)
    {
        return await _tripService.GetAll();
    }
}