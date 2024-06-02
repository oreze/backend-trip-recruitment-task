using BackendTripRecruitmentTask.Application.Dtos.Trips;
using BackendTripRecruitmentTask.Application.Services;
using BackendTripRecruitmentTask.Domain.Entities;
using BackendTripRecruitmentTask.Domain.Exceptions;
using BackendTripRecruitmentTask.Infrastructure.Data;
using BackendTripRecruitmentTask.UnitTests.Helpers;
using Moq;
using Moq.EntityFrameworkCore;

namespace BackendTripRecruitmentTask.UnitTests.ApplicationTests.ServicesTests;

/// <summary>
/// These tests are not entirely units, but I want to keep them here as a demonstration of how this service could be tested.
/// However this logic should be tested in IntegrationTests project
/// </summary>
public class TripServiceTests
{
    private readonly Mock<TripDbContext> _mockDbContext;
    private readonly ITripService _tripService;

    public TripServiceTests()
    {
        _mockDbContext = new Mock<TripDbContext>();
        _tripService = new TripService(_mockDbContext.Object);
    }

    [Fact]
    public async Task CreateTrip_CountryDoesNotExist_ThrowsInputException()
    {
        var createTripDto = new CreateTripDto(
            "Test Trip",
            "This is a test trip.",
            DateTime.UtcNow.AddDays(1),
            50,
            "Non-Existent Country");

        var countries = Enumerable.Empty<Country>();
        _mockDbContext
            .Setup(db => db.Countries)
            .ReturnsDbSet(countries);

        var exception = await Assert.ThrowsAsync<InputException>(() => _tripService.CreateTrip(createTripDto));
        Assert.Equal(nameof(createTripDto.CountryName), exception.ParamName);
        _mockDbContext.Verify();
    }

    [Fact]
    public async Task CreateTrip_TripWithNameExists_ThrowsInputException()
    {
        var createTripDto = new CreateTripDto(
            "Trip1",
            "This is a test trip.",
            DateTime.UtcNow.AddDays(1),
            50,
            "Poland");

        var existingCountry = (Country?)Activator.CreateInstance(typeof(Country), true);
        PropertyHelper.SetProperty(existingCountry, nameof(existingCountry.Name), "Poland");
        var countries = new List<Country> { existingCountry! };
        _mockDbContext.Setup(db => db.Countries)
            .ReturnsDbSet(countries);

        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
        PropertyHelper.SetProperty(trip, nameof(existingCountry.Name), "Trip1");
        var trips = new List<Trip> { trip! };
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var exception = await Assert.ThrowsAsync<InputException>(() => _tripService.CreateTrip(createTripDto));
        Assert.Equal(nameof(createTripDto.Name), exception.ParamName);
        _mockDbContext.Verify();
    }

    [Fact]
    public async Task CreateTrip_ValidInput_ReturnsTripId()
    {
        var createTripDto = new CreateTripDto(
            "Test Trip",
            "This is a test trip.",
            DateTime.UtcNow.AddDays(1),
            50,
            "Poland");

        var existingCountry = (Country?)Activator.CreateInstance(typeof(Country), true);
        PropertyHelper.SetProperty(existingCountry, nameof(existingCountry.Name), "Poland");
        var countries = new List<Country> { existingCountry! };
        _mockDbContext.Setup(db => db.Countries)
            .ReturnsDbSet(countries);

        var trips = new List<Trip>();
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        _mockDbContext.Setup(x => x.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
            .Callback((Trip model, CancellationToken _) =>
            {
                model.ID = 1;
                trips.Add(model);
            });

        var tripId = await _tripService.CreateTrip(createTripDto);

        Assert.Equal(1, tripId);
        _mockDbContext.Verify();
    }

    [Fact]
    public async Task EditTrip_TripDoesNotExist_ThrowsNotFoundException()
    {
        var editTripDto = new EditTripDto(
            Name: "New Trip Name",
            Description: "New Trip Description",
            StartDate: DateTime.UtcNow.AddDays(1),
            NumberOfSeats: 100,
            CountryName: null);

        var trips = Enumerable.Empty<Trip>();
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _tripService.EditTrip(1, editTripDto));
        Assert.Equal("Could not find trip with ID 1.", exception.Message);
        _mockDbContext.Verify();
    }

    [Fact]
    public async Task EditTrip_TripWithNameExists_ThrowsInputException()
    {
        var editTripDto = new EditTripDto(
            Name: "Existing Trip Name",
            Description: "New Trip Description",
            StartDate: DateTime.UtcNow.AddDays(1),
            NumberOfSeats: 100,
            CountryName: null);

        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
        PropertyHelper.SetProperty(trip, nameof(Trip.ID), 1);
        PropertyHelper.SetProperty(trip, nameof(Trip.Name), "Existing Trip Name");
        var trips = new List<Trip> { trip! };
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var exception = await Assert.ThrowsAsync<InputException>(() => _tripService.EditTrip(1, editTripDto));
        Assert.Equal(nameof(editTripDto.Name), exception.ParamName);
        _mockDbContext.Verify();
    }

    [Fact]
    public async Task EditTrip_CountryDoesNotExist_ThrowsInputException()
    {
        var editTripDto = new EditTripDto(
            Name: "New Trip Name",
            Description: "New Trip Description",
            StartDate: DateTime.UtcNow.AddDays(1),
            NumberOfSeats: 100,
            CountryName: "Random Country That Doesn't Exists");

        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
        PropertyHelper.SetProperty(trip, nameof(Country.Name), "Trip1");
        var trips = new List<Trip> { trip! };
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var countries = Enumerable.Empty<Country>();
        _mockDbContext.Setup(db => db.Countries)
            .ReturnsDbSet(countries);

        await Assert.ThrowsAsync<NotFoundException>(() => _tripService.EditTrip(1, editTripDto));
        _mockDbContext.Verify();
    }

    [Fact]
    public async Task EditTrip_NumberOfSeatsLowerThanRegisteredUsers_ThrowsInputException()
    {
        var editTripDto = new EditTripDto(
            Name: "New Trip Name",
            Description: "New Trip Description",
            StartDate: DateTime.UtcNow.AddDays(1),
            NumberOfSeats: 1,
            CountryName: null);

        var registrations = new List<Registration>();
        for (int i = 0; i < 10; i++)
        {
            var registration = (Registration?)Activator.CreateInstance(typeof(Registration), true);
            PropertyHelper.SetProperty(registration, nameof(Registration.TripID), 1);
            registrations.Add(registration!);
        }
        
        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
        PropertyHelper.SetProperty(trip, nameof(Trip.Name), "Trip1");
        PropertyHelper.SetProperty(trip, nameof(Trip.ID), 1);
        PropertyHelper.SetProperty(trip, nameof(Trip.Registrations), registrations);
        var trips = new List<Trip> { trip! };
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var exception = await Assert.ThrowsAsync<InputException>(() => _tripService.EditTrip(1, editTripDto));
        Assert.Equal(nameof(editTripDto.NumberOfSeats), exception.ParamName);
        _mockDbContext.Verify();
    }

    [Fact]
    public async Task EditTrip_ValidInput_UpdatesTrip()
    {
        var editTripDto = new EditTripDto(
            Name: "New Trip Name",
            Description: "New Trip Description",
            StartDate: DateTime.UtcNow.AddDays(1),
            NumberOfSeats: 1,
            CountryName: "England");

        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
        PropertyHelper.SetProperty(trip, nameof(Trip.Name), "Trip1");
        PropertyHelper.SetProperty(trip, nameof(Trip.ID), 1);
        var trips = new List<Trip> { trip! };
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var poland = (Country?)Activator.CreateInstance(typeof(Country), true);
        var england = (Country?)Activator.CreateInstance(typeof(Country), true);
        PropertyHelper.SetProperty(poland, nameof(Country.Name), "Poland");
        PropertyHelper.SetProperty(trip, nameof(Trip.Country), poland!);
        PropertyHelper.SetProperty(england, nameof(Country.Name), "England");
        var countries = new List<Country> { poland!, england! };
        _mockDbContext.Setup(db => db.Countries)
            .ReturnsDbSet(countries);

        await _tripService.EditTrip(1, editTripDto);

        Assert.Equal(editTripDto.Name, trip!.Name);
        Assert.Equal(editTripDto.Description, trip.Description);
        Assert.Equal(editTripDto.StartDate, trip.StartDate);
        Assert.Equal(editTripDto.NumberOfSeats, trip.NumberOfSeats);
        Assert.Equal(england!.Name, trip.Country.Name);
    }
}