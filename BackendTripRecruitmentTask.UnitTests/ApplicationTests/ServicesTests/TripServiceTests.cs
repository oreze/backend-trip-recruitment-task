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
///     These tests are not entirely units, but I want to keep them here as a demonstration of how this service could be
///     tested. 
///     This logic is also tested in IntegrationTests project
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

        _mockDbContext.Setup(x => x.Trips.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
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
            "New Trip Name",
            "New Trip Description",
            DateTime.UtcNow.AddDays(1),
            100,
            "Poland");

        var existingCountry = (Country?)Activator.CreateInstance(typeof(Country), true);
        PropertyHelper.SetProperty(existingCountry, nameof(existingCountry.Name), "Poland");
        var countries = new List<Country> { existingCountry! };
        _mockDbContext.Setup(db => db.Countries)
            .ReturnsDbSet(countries);

        var trips = Enumerable.Empty<Trip>();
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _tripService.EditTrip(1, editTripDto));
        Assert.Equal("Could not find trip with ID 1.", exception.Message);
        _mockDbContext.Verify();
    }

    [Fact]
    public async Task EditTrip_CanSetTheSameName_UpdateTrip()
    {
        var editTripDto = new EditTripDto(
            "Existing Trip Name",
            "New Trip Description",
            DateTime.UtcNow.AddDays(1),
            100,
            "Poland");

        var existingCountry = (Country?)Activator.CreateInstance(typeof(Country), true);
        PropertyHelper.SetProperty(existingCountry, nameof(existingCountry.Name), "Poland");
        var countries = new List<Country> { existingCountry! };
        _mockDbContext.Setup(db => db.Countries)
            .ReturnsDbSet(countries);

        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
        PropertyHelper.SetProperty(trip, nameof(Trip.ID), 1);
        PropertyHelper.SetProperty(trip, nameof(Trip.Name), "Existing Trip Name");
        var trips = new List<Trip> { trip! };
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        await _tripService.EditTrip(1, editTripDto);
        Assert.Equal(editTripDto.Name, trip!.Name);
        Assert.Equal(editTripDto.Description, trip.Description);
        Assert.Equal(editTripDto.StartDate, trip.StartDate);
        Assert.Equal(editTripDto.NumberOfSeats, trip.NumberOfSeats);
        _mockDbContext.Verify();
    }

    [Fact]
    public async Task EditTrip_TripWithNameExists_ThrowsInputException()
    {
        var editTripDto = new EditTripDto(
            "Trip2",
            "New Trip Description",
            DateTime.UtcNow.AddDays(1),
            100,
            null);

        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
        PropertyHelper.SetProperty(trip, nameof(Trip.ID), 1);
        PropertyHelper.SetProperty(trip, nameof(Trip.Name), "Existing Trip Name");

        var tripWithDifferentName = (Trip?)Activator.CreateInstance(typeof(Trip), true);
        PropertyHelper.SetProperty(tripWithDifferentName, nameof(Trip.ID), 2);
        PropertyHelper.SetProperty(tripWithDifferentName, nameof(Trip.Name), "Trip2");

        var trips = new List<Trip> { trip!, tripWithDifferentName! };
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
            "New Trip Name",
            "New Trip Description",
            DateTime.UtcNow.AddDays(1),
            100,
            "Random Country That Doesn't Exists");

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
            "New Trip Name",
            "New Trip Description",
            DateTime.UtcNow.AddDays(1),
            1,
            null);

        var registrations = new List<Registration>();
        for (var i = 0; i < 10; i++)
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
            "New Trip Name",
            "New Trip Description",
            DateTime.UtcNow.AddDays(1),
            1,
            "England");

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

    [Fact]
    public async Task DeleteTrip_TripDoesNotExist_ReturnsFalse()
    {
        var trips = Enumerable.Empty<Trip>();
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var result = await _tripService.DeleteTrip(1);

        Assert.False(result);
        _mockDbContext.Verify(db => db.RemoveRange(It.IsAny<IEnumerable<Registration>>()), Times.Never);
        _mockDbContext.Verify(db => db.Remove(It.IsAny<Trip>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTrip_TripExists_DeletesTripAndRegistrations()
    {
        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
        PropertyHelper.SetProperty(trip, nameof(Trip.ID), 1);
        var trips = new List<Trip> { trip! };
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var registrations = new List<Registration>();
        for (var i = 0; i < 10; i++)
        {
            var registration = (Registration?)Activator.CreateInstance(typeof(Registration), true);
            PropertyHelper.SetProperty(registration, nameof(Registration.TripID), 1);
            registrations.Add(registration!);
        }

        _mockDbContext.Setup(db => db.Registrations)
            .ReturnsDbSet(registrations);

        var result = await _tripService.DeleteTrip(1);

        Assert.True(result);
        _mockDbContext.Verify(
            db => db.RemoveRange(It.Is<IEnumerable<Registration>>(r => r.SequenceEqual(registrations))), Times.Once);
        _mockDbContext.Verify(db => db.Remove(It.Is<Trip>(t => t.ID == trip!.ID)), Times.Once);
        _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsAllTrips()
    {
        var trips = new List<Trip>();
        for (var i = 1; i < 11; i++)
        {
            var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
            PropertyHelper.SetProperty(trip, nameof(Trip.ID), i);
        }

        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var result = await _tripService.GetAll();

        Assert.Equal(trips.Count, result.Count());
        for (var i = 0; i < trips.Count; i++)
        {
            var trip = trips[i];
            Assert.Equal(trip.ID, ++i);
        }
    }

    [Fact]
    public async Task GetAll_NoTrips_ReturnsEmptyList()
    {
        var trips = Enumerable.Empty<Trip>();
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var result = await _tripService.GetAll();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByCountry_ReturnsTripsForCountry()
    {
        var poland = (Country?)Activator.CreateInstance(typeof(Country), true);
        var england = (Country?)Activator.CreateInstance(typeof(Country), true);
        PropertyHelper.SetProperty(poland, nameof(Country.Name), "Poland");
        PropertyHelper.SetProperty(england, nameof(Country.Name), "England");
        var countries = new List<Country> { poland!, england! };
        _mockDbContext.Setup(db => db.Countries)
            .ReturnsDbSet(countries);

        var trips = new List<Trip>();
        for (var i = 1; i < 11; i++)
        {
            var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
            PropertyHelper.SetProperty(trip, nameof(Trip.ID), i);
            PropertyHelper.SetProperty(trip, nameof(Trip.Country), i % 2 == 0 ? poland! : england!);
            trips.Add(trip!);
        }

        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var result = (await _tripService.GetByCountry(poland!.Name)).ToList();

        Assert.Equal(5, result.Count());
        Assert.True(result.All(x => x.Country == poland.Name));
    }

    [Fact]
    public async Task GetByCountry_NoTripsForCountry_ReturnsEmptyList()
    {
        var poland = (Country?)Activator.CreateInstance(typeof(Country), true);
        PropertyHelper.SetProperty(poland, nameof(Country.Name), "Poland");

        var trips = new List<Trip>();
        for (var i = 1; i < 11; i++)
        {
            var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
            PropertyHelper.SetProperty(trip, nameof(Trip.ID), i);
            PropertyHelper.SetProperty(trip, nameof(Trip.Country), poland!);
            trips.Add(trip!);
        }

        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var result = await _tripService.GetByCountry("England");

        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetDetails_TripDoesNotExist_ThrowsNotFoundException()
    {
        var trips = Enumerable.Empty<Trip>();
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _tripService.GetDetails(1));
        Assert.Equal("Could not find trip with ID 1.", exception.Message);
        _mockDbContext.Verify();
    }

    [Fact]
    public async Task GetDetails_TripExists_ReturnsTripDetailsDto()
    {
        var country = (Country?)Activator.CreateInstance(typeof(Country), true);
        PropertyHelper.SetProperty(country, nameof(Country.Name), "Poland");

        var registrations = new List<Registration>();
        for (var i = 0; i < 10; i++)
        {
            var registration = (Registration?)Activator.CreateInstance(typeof(Registration), true);
            PropertyHelper.SetProperty(registration, nameof(Registration.Email), $"user{i}@example.com");
            PropertyHelper.SetProperty(registration, nameof(Registration.RegisteredAt), DateTime.UtcNow.AddDays(-i));
            registrations.Add(registration!);
        }

        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
        PropertyHelper.SetProperty(trip, nameof(Trip.ID), 1);
        PropertyHelper.SetProperty(trip, nameof(Trip.Name), "Trip1");
        PropertyHelper.SetProperty(trip, nameof(Trip.Country), country!);
        PropertyHelper.SetProperty(trip, nameof(Trip.Description), "This is a test trip.");
        PropertyHelper.SetProperty(trip, nameof(Trip.StartDate), DateTime.UtcNow.AddDays(1));
        PropertyHelper.SetProperty(trip, nameof(Trip.NumberOfSeats), 50);
        PropertyHelper.SetProperty(trip, nameof(Trip.Registrations), registrations);

        var trips = new List<Trip> { trip! };
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var result = await _tripService.GetDetails(1);

        Assert.Equal(trip!.Name, result.Name);
        Assert.Equal(trip.Country.Name, result.Country);
        Assert.Equal(trip.Description, result.Description);
        Assert.Equal(trip.StartDate, result.StartDate);
        Assert.Equal(trip.NumberOfSeats, result.NumberOfSeats);
        Assert.Equal(trip.Registrations.Count, result.RegistrationDetails.Count());
        for (var i = 0; i < trip.Registrations.Count; i++)
        {
            var registration = trip.Registrations[i];
            var registrationDto = result.RegistrationDetails.ElementAt(i);
            Assert.Equal(registration.Email, registrationDto.Email);
            Assert.Equal(registration.RegisteredAt, registrationDto.RegisteredAt);
        }
        _mockDbContext.Verify();
    }
    
    [Fact]
    public async Task Register_TripDoesNotExist_ThrowsNotFoundException()
    {
        var trips = Enumerable.Empty<Trip>();
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _tripService.Register(1, "user@example.com"));
        Assert.Equal("Could not find trip with ID 1.", exception.Message);
        _mockDbContext.Verify();
    }

    [Fact]
    public async Task Register_RegistrationLimitExceeded_ThrowsTripRegistrationLimitExceededException()
    {
        var registrations = new List<Registration>();
        for (var i = 0; i < 50; i++)
        {
            var registration = (Registration?)Activator.CreateInstance(typeof(Registration), true);
            registrations.Add(registration!);
        }

        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
        PropertyHelper.SetProperty(trip, nameof(Trip.ID), 1);
        PropertyHelper.SetProperty(trip, nameof(Trip.NumberOfSeats), 50);
        PropertyHelper.SetProperty(trip, nameof(Trip.Registrations), registrations);

        var trips = new List<Trip> { trip! };
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var exception = await Assert.ThrowsAsync<TripRegistrationLimitExceededException>(() => _tripService.Register(1, "user@example.com"));
        Assert.Equal($"The registration limit for trip with ID 1 has been exceeded.Number of seats is 50.", exception.Message);
        _mockDbContext.Verify();
    }

    [Fact]
    public async Task Register_EmailAlreadyRegistered_ThrowsInputException()
    {
        var registrations = new List<Registration>();
        var registration = (Registration?)Activator.CreateInstance(typeof(Registration), true);
        PropertyHelper.SetProperty(registration, nameof(Registration.Email), "user@example.com");
        registrations.Add(registration!);

        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
        PropertyHelper.SetProperty(trip, nameof(Trip.ID), 1);
        PropertyHelper.SetProperty(trip, nameof(Trip.NumberOfSeats), 50);
        PropertyHelper.SetProperty(trip, nameof(Trip.Registrations), registrations);

        var trips = new List<Trip> { trip! };
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);

        var exception = await Assert.ThrowsAsync<InputException>(() => _tripService.Register(1, "user@example.com"));
        Assert.True(nameof(registration.Email).Equals(exception.ParamName, StringComparison.InvariantCultureIgnoreCase));
        Assert.StartsWith($"The email address user@example.com is already registered for trip with ID 1.", exception.Message);
        _mockDbContext.Verify();
    }

    [Fact]
    public async Task Register_ValidInput_AddsRegistration()
    {
        var registrations = new List<Registration>();

        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);
        PropertyHelper.SetProperty(trip, nameof(Trip.ID), 1);
        PropertyHelper.SetProperty(trip, nameof(Trip.NumberOfSeats), 50);
        PropertyHelper.SetProperty(trip, nameof(Trip.Registrations), registrations);

        var trips = new List<Trip> { trip! };
        _mockDbContext.Setup(db => db.Trips)
            .ReturnsDbSet(trips);
        
        _mockDbContext.Setup(db => db.Registrations)
            .ReturnsDbSet(registrations);
        _mockDbContext.Setup(x => x.Registrations.AddAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()))
            .Callback((Registration model, CancellationToken _) =>
            {
                model.ID = 1;
                registrations.Add(model);
            });
        
        await _tripService.Register(1, "user@example.com");

        Assert.Single(registrations);
        Assert.Equal("user@example.com", registrations[0].Email);
        Assert.Equal(trip, registrations[0].Trip);
        _mockDbContext.Verify();
    }
}