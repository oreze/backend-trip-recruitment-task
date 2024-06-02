using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BackendTripRecruitmentTask.Application.Dtos.Trips;
using BackendTripRecruitmentTask.Domain.Entities;
using BackendTripRecruitmentTask.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTripRecruitmentTask.IntegrationTests.Features;

//TODO: Refactor, use dbcontext instead of controller actions for seeding/validating
public class TripTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly TripDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly IServiceScope _scope;

    public TripTests(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateDefaultClient();
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<TripDbContext>();
    }

    ~TripTests()
    {
        _dbContext.Dispose();
        _scope.Dispose();
    }

    [Fact]
    public async Task CreateTrip_ValidInput_ReturnsOk()
    {
        var createTripDto = new CreateTripDto(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            "Poland");

        var json = JsonSerializer.Serialize(createTripDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/trips", content);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<int>();
        Assert.NotEqual(0, result);

        var trip = await _dbContext.Trips
            .Include(x => x.Country)
            .FirstOrDefaultAsync(x => x.ID == result);
        Assert.NotNull(trip);
        Assert.Equal(createTripDto.Name, trip!.Name);
        Assert.Equal(createTripDto.Description, trip!.Description);
        Assert.Equal(createTripDto.CountryName, trip!.Country.Name);
        Assert.Equal(createTripDto.NumberOfSeats, trip!.NumberOfSeats);
        Assert.Equal(createTripDto.StartDate, trip!.StartDate);
    }

    [Fact]
    public async Task CreateTrip_TripWithNameExists_ReturnsBadRequest()
    {
        var randomTripName = Guid.NewGuid().ToString();
        var createTripDto = new CreateTripDto(
            randomTripName,
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            "Poland");

        var createTripWithSameNameDto = new CreateTripDto(
            randomTripName,
            "Random description2",
            DateTime.UtcNow.AddDays(5),
            10,
            "Poland");

        var json = JsonSerializer.Serialize(createTripDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/trips", content);

        response.EnsureSuccessStatusCode();

        var secondTripJson = JsonSerializer.Serialize(createTripWithSameNameDto);
        var secondTripContent = new StringContent(secondTripJson, Encoding.UTF8, "application/json");
        var secondTripResponse = await _httpClient.PostAsync("/trips", secondTripContent);

        Assert.Equal(HttpStatusCode.BadRequest, secondTripResponse.StatusCode);
    }

    [Fact]
    public async Task CreateTrip_CountryDoesNotExists_ReturnsBadRequest()
    {
        var createTripDto = new CreateTripDto(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            "Random Country That Does Not Exists");

        var json = JsonSerializer.Serialize(createTripDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/trips", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTrip_InvalidInput_ReturnsBadRequest()
    {
        var createTripDto = new CreateTripDto(
            "",
            "",
            DateTime.UtcNow.AddDays(10),
            2137,
            "Poland");

        var json = JsonSerializer.Serialize(createTripDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/trips", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EditTrip_ValidInput_ReturnsOk()
    {
        var createTripDto = new CreateTripDto(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            "Poland");

        var editTripDto = new EditTripDto(
            Guid.NewGuid().ToString(),
            "This is an updated test trip.",
            DateTime.UtcNow.AddDays(2),
            100,
            "Germany");

        var createTripJson = JsonSerializer.Serialize(createTripDto);
        var createTripContent = new StringContent(createTripJson, Encoding.UTF8, "application/json");
        var createTripResponse = await _httpClient.PostAsync("/trips", createTripContent);
        createTripResponse.EnsureSuccessStatusCode();
        var tripID = await createTripResponse.Content.ReadFromJsonAsync<int>();
        Assert.NotEqual(0, tripID);

        var editTripJson = JsonSerializer.Serialize(editTripDto);
        var editTripContnet = new StringContent(editTripJson, Encoding.UTF8, "application/json");
        var editTripResponse = await _httpClient.PutAsync($"/trips/{tripID}", editTripContnet);
        editTripResponse.EnsureSuccessStatusCode();

        var updatedTrip = await _dbContext.Trips.Include(x => x.Country).FirstOrDefaultAsync(x => x.ID == tripID);
        Assert.NotNull(updatedTrip);
        Assert.Equal(editTripDto.Name, updatedTrip.Name);
        Assert.Equal(editTripDto.Description, updatedTrip.Description);
        Assert.Equal(editTripDto.StartDate, updatedTrip.StartDate);
        Assert.Equal(editTripDto.NumberOfSeats, updatedTrip.NumberOfSeats);
        Assert.Equal(editTripDto.CountryName, updatedTrip.Country.Name);
    }

    [Fact]
    public async Task EditTrip_TripDoesNotExist_ReturnsNotFound()
    {
        var editTripDto = new EditTripDto(
            "Updated Trip",
            "This is an updated test trip.",
            DateTime.UtcNow.AddDays(2),
            100,
            "England");

        var json = JsonSerializer.Serialize(editTripDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync("/trips/999", content);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task EditTrip_TripWithNameExists_ReturnsBadRequest()
    {
        var tripName = Guid.NewGuid().ToString();
        var createTripDto = new CreateTripDto(
            tripName,
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            "Poland");

        var createTripWithDifferentNameDto = new CreateTripDto(
            tripName + "1",
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            "Poland");

        var editTripDto = new EditTripDto(
            tripName + "1",
            "This is an updated test trip.",
            DateTime.UtcNow.AddDays(2),
            100,
            "Germany");

        var createTripJson = JsonSerializer.Serialize(createTripDto);
        var createTripContent = new StringContent(createTripJson, Encoding.UTF8, "application/json");
        var createTripResponse = await _httpClient.PostAsync("/trips", createTripContent);
        createTripResponse.EnsureSuccessStatusCode();
        var tripID = await createTripResponse.Content.ReadFromJsonAsync<int>();
        Assert.NotEqual(0, tripID);

        var createTripWithDifferentNameJson = JsonSerializer.Serialize(createTripWithDifferentNameDto);
        var createTripWithDifferentNameContent =
            new StringContent(createTripWithDifferentNameJson, Encoding.UTF8, "application/json");
        var createTripWithDifferentNameResponse =
            await _httpClient.PostAsync("/trips", createTripWithDifferentNameContent);
        createTripWithDifferentNameResponse.EnsureSuccessStatusCode();

        var editTripJson = JsonSerializer.Serialize(editTripDto);
        var editTripContnet = new StringContent(editTripJson, Encoding.UTF8, "application/json");
        var editTripResponse = await _httpClient.PutAsync($"/trips/{tripID}", editTripContnet);
        Assert.Equal(HttpStatusCode.BadRequest, editTripResponse.StatusCode);

        var updatedTrip = await _dbContext.Trips.Include(x => x.Country).FirstOrDefaultAsync(x => x.ID == tripID);
        Assert.NotNull(updatedTrip);
        Assert.Equal(createTripDto.Name, updatedTrip.Name);
        Assert.Equal(createTripDto.Description, updatedTrip.Description);
        Assert.Equal(createTripDto.StartDate, updatedTrip.StartDate);
        Assert.Equal(createTripDto.NumberOfSeats, updatedTrip.NumberOfSeats);
        Assert.Equal(createTripDto.CountryName, updatedTrip.Country.Name);
    }


    [Fact]
    public async Task EditTrip_CountryDoesNotExist_ReturnsBadRequest()
    {
        var createTripDto = new CreateTripDto(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            "Poland");

        var editTripDto = new EditTripDto(
            Guid.NewGuid().ToString(),
            "This is an updated test trip.",
            DateTime.UtcNow.AddDays(2),
            100,
            "Non-Existent Country");

        var createTripJson = JsonSerializer.Serialize(createTripDto);
        var createTripContent = new StringContent(createTripJson, Encoding.UTF8, "application/json");
        var createTripResponse = await _httpClient.PostAsync("/trips", createTripContent);
        createTripResponse.EnsureSuccessStatusCode();
        var tripID = await createTripResponse.Content.ReadFromJsonAsync<int>();
        Assert.NotEqual(0, tripID);

        var json = JsonSerializer.Serialize(editTripDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/trips/{tripID}", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EditTrip_NumberOfSeatsLowerThanRegisteredUsers_ReturnsBadRequest()
    {
        var createTripDto = new CreateTripDto(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            "Poland");

        var editTripDto = new EditTripDto(
            Guid.NewGuid().ToString(),
            "This is an updated test trip.",
            DateTime.UtcNow.AddDays(2),
            1,
            "Germany");

        var createTripJson = JsonSerializer.Serialize(createTripDto);
        var createTripContent = new StringContent(createTripJson, Encoding.UTF8, "application/json");
        var createTripResponse = await _httpClient.PostAsync("/trips", createTripContent);
        createTripResponse.EnsureSuccessStatusCode();
        var tripID = await createTripResponse.Content.ReadFromJsonAsync<int>();
        Assert.NotEqual(0, tripID);

        for (var i = 0; i < 5; i++)
        {
            var registerForTripResponse =
                await _httpClient.PostAsync($"/trips/{tripID}/register?email=test{i}@test.com", null);
            registerForTripResponse.EnsureSuccessStatusCode();
        }

        var json = JsonSerializer.Serialize(editTripDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/trips/{tripID}", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTrip_TripExists_ReturnsOk()
    {
        var createTripDto = new CreateTripDto(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            "Poland");

        var createTripJson = JsonSerializer.Serialize(createTripDto);
        var createTripContent = new StringContent(createTripJson, Encoding.UTF8, "application/json");
        var createTripResponse = await _httpClient.PostAsync("/trips", createTripContent);
        createTripResponse.EnsureSuccessStatusCode();
        var tripID = await createTripResponse.Content.ReadFromJsonAsync<int>();
        Assert.NotEqual(0, tripID);

        var deleteTripResponse = await _httpClient.DeleteAsync($"/trips/{tripID}");

        deleteTripResponse.EnsureSuccessStatusCode();
        var deletedTrip = await _dbContext.Trips.FindAsync(tripID);
        Assert.Null(deletedTrip);
    }

    [Fact]
    public async Task DeleteTrip_TripDoesNotExist_ReturnsNotFound()
    {
        var response = await _httpClient.DeleteAsync($"/trips/{int.MaxValue}");
        response.EnsureSuccessStatusCode();
        var wasDeleted = await response.Content.ReadFromJsonAsync<bool>();
        Assert.False(wasDeleted);
    }

    [Fact]
    public async Task ListAllTrips_ReturnsAllTrips()
    {
        var country = _dbContext.Countries.First(x => x.ThreeLetterCode == "POL");
        var firstTrip = Trip.Create(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            country);
        
        var secondTrip = Trip.Create(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            country);

        await _dbContext.AddRangeAsync([firstTrip, secondTrip]);
        await _dbContext.SaveChangesAsync();

        var response = await _httpClient.GetAsync("/trips");

        response.EnsureSuccessStatusCode();
        var trips = await response.Content.ReadFromJsonAsync<List<TripListDto>>();
        Assert.True(trips!.Count >= 2);
        Assert.Contains(trips,
            t => t.Name == firstTrip.Name && t.Country == firstTrip.Country.Name &&
                 t.StartDate == firstTrip.StartDate);
        Assert.Contains(trips,
            t => t.Name == secondTrip.Name && t.Country == secondTrip.Country.Name &&
                 t.StartDate == secondTrip.StartDate);
    }

    [Fact]
    public async Task ListAllTrips_NoTrips_ReturnsEmptyList()
    {
        // Not the best solution, but I cannot make it work otherwise - database is not being cleaned up after each test.
        // EnsureDeleted/EnsureCreated don't work, it's something that should be taken care of later
        await _dbContext.Database.EnsureDeletedAsync();
        var response = await _httpClient.GetAsync("/trips");

        response.EnsureSuccessStatusCode();
        var trips = await response.Content.ReadFromJsonAsync<List<TripListDto>>();
        Assert.Empty(trips!);
    }

    [Fact]
    public async Task SearchTripsByCountry_ReturnsTripsForCountry()
    {
        var country = _dbContext.Countries.First(x => x.ThreeLetterCode == "POL");
        var firstTrip = Trip.Create(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            country);
        
        var secondTrip = Trip.Create(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            country);

        await _dbContext.AddRangeAsync([firstTrip, secondTrip]);
        await _dbContext.SaveChangesAsync();
        
        var response = await _httpClient.GetAsync("/trips/country/Poland");

        response.EnsureSuccessStatusCode();
        var trips = await response.Content.ReadFromJsonAsync<List<TripSearchDto>>();
        Assert.True(trips!.Count >= 2);
        Assert.Contains(trips,
            t => t.Name == firstTrip.Name && t.Country == firstTrip.Country.Name &&
                 t.StartDate == firstTrip.StartDate);
        Assert.Contains(trips,
            t => t.Name == secondTrip.Name && t.Country == secondTrip.Country.Name &&
                 t.StartDate == secondTrip.StartDate);
    }

    [Fact]
    public async Task SearchTripsByCountry_NoTripsForCountry_ReturnsEmptyList()
    {
        var newCountry = Country.Create("XXX", "NotExistingCountry");
        await _dbContext.Countries.AddAsync(Country.Create("XXX", "NotExistingCountry"));
        await _dbContext.SaveChangesAsync();

        var response = await _httpClient.GetAsync($"/trips/country/{newCountry.Name}");

        response.EnsureSuccessStatusCode();
        var trips = await response.Content.ReadFromJsonAsync<List<TripSearchDto>>();
        Assert.Empty(trips!);
    }

    [Fact]
    public async Task GetSingleTrip_TripExists_ReturnsTrip()
    {
        var country = _dbContext.Countries.First(x => x.ThreeLetterCode == "POL");
        var trip = Trip.Create(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            country);

        await _dbContext.Trips.AddAsync(trip);

        for (var i = 0; i < 5; i++)
        {
            var registration = Registration.Create("test{i}@test.com", trip);
            await _dbContext.Registrations.AddAsync(registration);
        }
        await _dbContext.SaveChangesAsync();

        var response = await _httpClient.GetAsync($"/trips/{trip.ID}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TripDetailsDto>();
        Assert.NotNull(result);
        Assert.Equal(trip.Name, result.Name);
        Assert.Equal(trip.Description, result.Description);
        Assert.Equal(trip.StartDate, result.StartDate);
        Assert.Equal(trip.NumberOfSeats, result.NumberOfSeats);
        Assert.Equal(trip.Country.Name, result.Country);
        Assert.Equal(5, result.RegistrationDetails.Count());
    }

    [Fact]
    public async Task GetSingleTrip_TripDoesNotExist_ReturnsNotFound()
    {
        var response = await _httpClient.GetAsync($"/trips/{int.MaxValue}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RegisterForTrip_TripExists_ReturnsOk()
    {
        var country = _dbContext.Countries.First(x => x.ThreeLetterCode == "POL");
        var trip = Trip.Create(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            country);

        await _dbContext.Trips.AddAsync(trip);
        await _dbContext.SaveChangesAsync();

        const string email = "test@example.com";

        var response = await _httpClient.PostAsync($"/trips/{trip.ID}/register?email={email}", null);

        response.EnsureSuccessStatusCode();
        var registration =
            await _dbContext.Registrations.FirstOrDefaultAsync(r => r.TripID == trip.ID && r.Email == email);
        Assert.NotNull(registration);
    }

    [Fact]
    public async Task RegisterForTrip_TripDoesNotExist_ReturnsNotFound()
    {
        const string email = "test@example.com";

        var response = await _httpClient.PostAsync($"/trips/{int.MaxValue}/register?email={email}", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RegisterForTrip_InvalidEmail_ReturnsBadRequest()
    {
        var country = _dbContext.Countries.First(x => x.ThreeLetterCode == "POL");
        var trip = Trip.Create(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            country);

        await _dbContext.Trips.AddAsync(trip);
        await _dbContext.SaveChangesAsync();

        const string email = "invalid-email";

        var response = await _httpClient.PostAsync($"/trips/{trip.ID}/register?email={email}", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RegisterForTrip_EmailAlreadyRegistered_ReturnsBadRequest()
    {
        var country = _dbContext.Countries.First(x => x.ThreeLetterCode == "POL");
        var trip = Trip.Create(
            Guid.NewGuid().ToString(),
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50,
            country);

        await _dbContext.Trips.AddAsync(trip);
        
        const string email = "test@example.com";

        var registration = Registration.Create(email, trip);
        _dbContext.Registrations.Add(registration);
        await _dbContext.SaveChangesAsync();

        var response = await _httpClient.PostAsync($"/trips/{trip.ID}/register?email={email}", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}