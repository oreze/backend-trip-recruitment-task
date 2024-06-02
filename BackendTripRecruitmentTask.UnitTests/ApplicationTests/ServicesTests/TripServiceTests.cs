using BackendTripRecruitmentTask.Application.Dtos.Trips;
using BackendTripRecruitmentTask.Application.Services;
using BackendTripRecruitmentTask.Domain.Entities;
using BackendTripRecruitmentTask.Domain.Exceptions;
using BackendTripRecruitmentTask.Infrastructure.Data;
using BackendTripRecruitmentTask.UnitTests.Helpers;
using Moq;
using Moq.EntityFrameworkCore;

namespace BackendTripRecruitmentTask.UnitTests.ApplicationTests.ServicesTests;

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
}