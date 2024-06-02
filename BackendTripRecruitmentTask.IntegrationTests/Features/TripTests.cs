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
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public TripTests(WebApplicationFactory<Program> factory)
    {
        _webApplicationFactory = factory;
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
            "Trip1",
            "Random description",
            DateTime.UtcNow.AddDays(10),
            50, 
            "Poland");

        var json = JsonSerializer.Serialize(createTripDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/trips", content);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<int>();
        Assert.Equal(1, result);

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
}