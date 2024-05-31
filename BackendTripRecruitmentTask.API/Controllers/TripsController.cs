using BackendTripRecruitmentTask.Application.Commands;
using BackendTripRecruitmentTask.Application.Dtos.Trips;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackendTripRecruitmentTask.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TripsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    
    [HttpPost]
    public async Task<IActionResult> CreateTrip(CreateTripDto createTripDto)
    {
        var command = new CreateTripCommand(createTripDto);
        int result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> EditTrip(int id, EditTripDto editTripDto)
    {
        throw new NotImplementedException();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrip(int id)
    {
        throw new NotImplementedException();
    }
    
    // GET api/trip
    [HttpGet]
    public async Task<IActionResult> ListAllTrips()
    {
        throw new NotImplementedException();
    }

    // GET api/trip/country/{country}
    [HttpGet("country/{country}")]
    public async Task<IActionResult> SearchTripsByCountry(string country)
    {
        throw new NotImplementedException();
    }

    // GET api/trip/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleTrip(int id)
    {
        throw new NotImplementedException();
    }

    // POST api/trip/{id}/register
    [HttpPost("{id}/register")]
    public async Task<IActionResult> RegisterForTrip(int id, string email)
    {
        throw new NotImplementedException();
    }
}