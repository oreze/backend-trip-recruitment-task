using BackendTripRecruitmentTask.Application.Dtos.Trips;

namespace BackendTripRecruitmentTask.Application.Services;

/// <summary>
///     Interface for managing trips in the travel application.
/// </summary>
public interface ITripService
{
    /// <summary>
    ///     Creates a new trip based on the data provided in a CreateTripDto object.
    /// </summary>
    /// <param name="createTripDto">The data for the new trip.</param>
    /// <returns>The ID of the newly created trip.</returns>
    public Task<int> CreateTrip(CreateTripDto createTripDto);

    /// <summary>
    ///     Edits an existing trip based on the data provided in an EditTripDto object.
    /// </summary>
    /// <param name="id">The ID of the trip to edit.</param>
    /// <param name="editTripDto">The new data for the trip.</param>
    public Task EditTrip(int id, EditTripDto editTripDto);

    /// <summary>
    ///     Deletes an existing trip.
    /// </summary>
    /// <param name="id">The ID of the trip to delete.</param>
    /// <returns>True if the trip was deleted successfully, false otherwise.</returns>
    public Task<bool> DeleteTrip(int id);

    /// <summary>
    ///     Retrieves a list of all trips in the database.
    /// </summary>
    /// <returns>A list of TripListDto objects containing the name, country name, and start date of each trip.</returns>
    public Task<IEnumerable<TripListDto>> GetAll();

    /// <summary>
    ///     Retrieves a list of all trips in a specific country.
    /// </summary>
    /// <param name="country">The name of the country.</param>
    /// <returns>A list of TripSearchDto objects containing the name, country name, and start date of each trip in the country.</returns>
    public Task<IEnumerable<TripSearchDto>> GetByCountry(string country);

    /// <summary>
    ///     Retrieves detailed information about a specific trip.
    /// </summary>
    /// <param name="id">The ID of the trip.</param>
    /// <returns>
    ///     A TripDetailsDto object containing the name, country name, description, start date, number of seats, and a
    ///     list of registrations for the trip.
    /// </returns>
    public Task<TripDetailsDto> GetDetails(int id);

    /// <summary>
    ///     Allows users to register for a trip.
    /// </summary>
    /// <param name="tripID">The ID of the trip.</param>
    /// <param name="email">The email address of the user.</param>
    public Task Register(int tripID, string email);
}