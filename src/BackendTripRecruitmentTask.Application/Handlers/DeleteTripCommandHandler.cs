using BackendTripRecruitmentTask.Application.Commands;
using BackendTripRecruitmentTask.Application.Services;
using MediatR;

namespace BackendTripRecruitmentTask.Application.Handlers;

public class DeleteTripCommandHandler(ITripService tripService) : IRequestHandler<DeleteTripCommand, bool>
{
    private readonly ITripService _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));

    public async Task<bool> Handle(DeleteTripCommand request, CancellationToken cancellationToken)
    {
        return await _tripService.DeleteTrip(request.ID);
    }
}