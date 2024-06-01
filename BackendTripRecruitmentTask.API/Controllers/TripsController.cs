using BackendTripRecruitmentTask.Application.Commands;
using BackendTripRecruitmentTask.Application.Dtos.Trips;
using BackendTripRecruitmentTask.Application.Queries;
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
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditTrip(int id, EditTripDto editTripDto)
    {
        var query = new EditTripCommand(id, editTripDto);
        await _mediator.Send(query);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrip(int id)
    {
        var command = new DeleteTripCommand(id);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    // GET api/trip
    [HttpGet]
    public async Task<IActionResult> ListAllTrips()
    {
        var query = new GetAllTripsQuery();
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    // GET api/trip/country/{country}
    [HttpGet("country/{country}")]
    public async Task<IActionResult> SearchTripsByCountry(string country)
    {
        var query = new SearchTripsByCountryQuery(country);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    // GET api/trip/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleTrip(int id)
    {
        var query = new GetSingleTripQuery(id);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    // POST api/trip/{id}/register
    [HttpPost("{id}/register")]
    public async Task<IActionResult> RegisterForTrip(int id, string email)
    {
        var command = new RegisterForTripCommand(id, email);
        await _mediator.Send(command);

        return Ok();
    }
}