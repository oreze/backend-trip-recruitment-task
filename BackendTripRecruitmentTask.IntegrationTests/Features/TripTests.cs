using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BackendTripRecruitmentTask.Application.Dtos.Trips;
using BackendTripRecruitmentTask.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTripRecruitmentTask.IntegrationTests.Features;

public class TripTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly TripDbContext _dbContext;
    private readonly IServiceScope _scope;
    private readonly HttpClient _httpClient;

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
            Name: Guid.NewGuid().ToString(),
            Description: "This is an updated test trip.",
            StartDate: DateTime.UtcNow.AddDays(2),
            NumberOfSeats: 100,
            CountryName: "Germany");

        var createTripJson = JsonSerializer.Serialize(createTripDto);
        var createTripContent = new StringContent(createTripJson, Encoding.UTF8, "application/json");
        var createTripResponse = await _httpClient.PostAsync($"/trips", createTripContent);
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
            Name: "Updated Trip",
            Description: "This is an updated test trip.",
            StartDate: DateTime.UtcNow.AddDays(2),
            NumberOfSeats: 100,
            CountryName: "England");

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

        var editTripDto = new EditTripDto(
            Name: tripName,
            Description: "This is an updated test trip.",
            StartDate: DateTime.UtcNow.AddDays(2),
            NumberOfSeats: 100,
            CountryName: "Germany");

        var createTripJson = JsonSerializer.Serialize(createTripDto);
        var createTripContent = new StringContent(createTripJson, Encoding.UTF8, "application/json");
        var createTripResponse = await _httpClient.PostAsync($"/trips", createTripContent);
        createTripResponse.EnsureSuccessStatusCode();
        var tripID = await createTripResponse.Content.ReadFromJsonAsync<int>();
        Assert.NotEqual(0, tripID);

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
}