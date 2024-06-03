using BackendTripRecruitmentTask.Application.Commands;
using BackendTripRecruitmentTask.Application.Services;
using MediatR;

namespace BackendTripRecruitmentTask.Application.Handlers;

public class RegisterForTripCommandHandler(ITripService tripService)
    : IRequestHandler<RegisterForTripCommand>
{
    private readonly ITripService _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));

    public async Task Handle(RegisterForTripCommand request, CancellationToken cancellationToken)
    {
        await _tripService.Register(request.ID, request.Email);
    }
}