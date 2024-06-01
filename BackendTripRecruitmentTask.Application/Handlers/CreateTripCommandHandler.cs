using BackendTripRecruitmentTask.Application.Commands;
using BackendTripRecruitmentTask.Application.Services;
using MediatR;

namespace BackendTripRecruitmentTask.Application.Handlers;

public class CreateTripCommandHandler(ITripService tripService): IRequestHandler<CreateTripCommand, int>
{
    private readonly ITripService _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));
    
    public async Task<int> Handle(CreateTripCommand request, CancellationToken cancellationToken) =>
        await _tripService.CreateTrip(request.CreateTripDto);
}