using BackendTripRecruitmentTask.Application.Commands;
using BackendTripRecruitmentTask.Application.Dtos.Trips;
using BackendTripRecruitmentTask.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackendTripRecruitmentTask.API.Controllers;

/// <summary>
///     Controller for managing trips.
/// </summary>
/// <param name="mediator">MediatR mediator implementation.</param>
[ApiController]
[Route("[controller]")]
public class TripsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    /// <summary>
    ///     Creates a new trip with the specified details.
    /// </summary>
    /// <param name="createTripDto">The data transfer object containing the trip details.</param>
    /// <returns>The ID of the created trip.</returns>
    /// <response code="200">Returns the ID of the created trip.</response>
    /// <response code="400">
    ///     Returns bad request if the provided country name does not exist in the database or if a trip with
    ///     the specified name already exists.
    /// </response>
    [HttpPost]
    public async Task<IActionResult> CreateTrip(CreateTripDto createTripDto)
    {
        var command = new CreateTripCommand(createTripDto);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    /// <summary>
    ///     Edits an existing trip with the specified details.
    /// </summary>
    /// <param name="id">The ID of the trip to be edited.</param>
    /// <param name="editTripDto">The data transfer object containing the new trip details.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Returns ok if the trip is successfully edited.</response>
    /// <response code="400">
    ///     Returns bad request if the provided country name does not exist in the database,
    ///     if a trip with the specified name already exists,
    ///     or if the number of seats is lower than the number of already registered people.
    /// </response>
    /// <response code="404">Returns not found if the trip with the specified ID does not exist.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> EditTrip(int id, EditTripDto editTripDto)
    {
        var query = new EditTripCommand(id, editTripDto);
        await _mediator.Send(query);

        return Ok();
    }

    /// <summary>
    ///     Deletes an existing trip with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the trip to be deleted.</param>
    /// <returns>True if the trip is successfully deleted, false otherwise.</returns>
    /// <response code="200">Returns true if trip was removed, false if not exists.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrip(int id)
    {
        var command = new DeleteTripCommand(id);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    /// <summary>
    ///     Retrieves a list of all trips.
    /// </summary>
    /// <returns>A list of trips with their name, country name, and start date.</returns>
    /// <response code="200">Returns a list of trips with their name, country name, and start date.</response>
    [HttpGet]
    public async Task<IActionResult> ListAllTrips()
    {
        var query = new GetAllTripsQuery();
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    ///     Searches for trips by country.
    /// </summary>
    /// <param name="country">The name of the country to search for.</param>
    /// <returns>A list of trips with their name, country name, and start date.</returns>
    /// <response code="200">Returns a list of trips with their name, country name, and start date.</response>
    [HttpGet("country/{country}")]
    public async Task<IActionResult> SearchTripsByCountry(string country)
    {
        var query = new SearchTripsByCountryQuery(country);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    ///     Retrieves details of a single trip with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the trip to be retrieved.</param>
    /// <returns>
    ///     Details of the trip, including its name, country name, description, start date, number of seats, and a list of
    ///     registrations with their email and registration date.
    /// </returns>
    /// <response code="200">Returns details of the trip.</response>
    /// <response code="404">Returns not found if the trip with the specified ID does not exist.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleTrip(int id)
    {
        var query = new GetSingleTripQuery(id);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    ///     Registers a user for a trip with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the trip to register for.</param>
    /// <param name="email">The email address of the user.</param>
    /// <returns>No content if the registration is successful.</returns>
    /// <response code="204">Returns no content if the registration is successful.</response>
    /// <response code="404">Returns not found if the trip with the specified ID does not exist.</response>
    /// <response code="400">
    ///     Returns bad request if the registration limit for the trip has been exceeded or if the email
    ///     address is already registered for the trip.
    /// </response>
    [HttpPost("{id}/register")]
    public async Task<IActionResult> RegisterForTrip(int id, string email)
    {
        var command = new RegisterForTripCommand(id, email);
        await _mediator.Send(command);

        return Ok();
    }
}