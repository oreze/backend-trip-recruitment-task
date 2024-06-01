using BackendTripRecruitmentTask.Application.Commands;
using BackendTripRecruitmentTask.Application.Services;
using MediatR;

namespace BackendTripRecruitmentTask.Application.Handlers;

public class EditTripCommandHandler(ITripService tripService) : IRequestHandler<EditTripCommand>
{
    private readonly ITripService _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));

    public async Task Handle(EditTripCommand request, CancellationToken cancellationToken)
    {
        await _tripService.EditTrip(request.ID, request.EditTripDto);
    }
}