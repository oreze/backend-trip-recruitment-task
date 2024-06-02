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
}