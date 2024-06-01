using BackendTripRecruitmentTask.Application.Dtos.Trips;
using BackendTripRecruitmentTask.Domain.Entities;
using BackendTripRecruitmentTask.Domain.Exceptions;
using BackendTripRecruitmentTask.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BackendTripRecruitmentTask.Application.Services;

public class TripService(TripDbContext dbContext) : ITripService
{
    private readonly TripDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<int> CreateTrip(CreateTripDto createTripDto)
    {
        var country = await _dbContext.Countries.FirstOrDefaultAsync(x =>
            x.Name.Equals(createTripDto.CountryName, StringComparison.InvariantCultureIgnoreCase));

        if (country == default)
            throw new InputException(nameof(createTripDto.CountryName),
                $"The country name '{createTripDto.CountryName}' does not exist in the database. " +
                $"Try using only English names, for reference check ISO 3166.");

        var doesTripWithNameExists = await _dbContext.Trips.AnyAsync(x =>
            x.Name.Equals(createTripDto.Name, StringComparison.InvariantCultureIgnoreCase));

        if (doesTripWithNameExists)
            throw new InputException(nameof(createTripDto.Name),
                $"The trip with name '{createTripDto.Name}' already exists in the database. " +
                $"Try using other name.");

        var trip = Trip.Create(createTripDto.Name, createTripDto.Description, createTripDto.StartDate,
            createTripDto.NumberOfSeats, country);
        await _dbContext.AddAsync(trip);
        await _dbContext.SaveChangesAsync();

        return trip.ID;
    }

    public async Task<bool> DeleteTrip(int ID)
    {
        var registrationsForTrip = _dbContext.Registrations
            .Where(x => x.TripID == ID);

        _dbContext.RemoveRange(registrationsForTrip);

        var tripToBeDeleted = await _dbContext.Trips.FirstOrDefaultAsync(x => x.ID == ID);

        if (tripToBeDeleted == default)
            return false;

        _dbContext.Remove(tripToBeDeleted);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TripListDto>> GetAll()
    {
        return await _dbContext.Trips
            .Include(x => x.Country)
            .Select(x => new TripListDto(x.Name, x.Country.Name, x.StartDate))
            .ToListAsync();
    }

    public async Task<IEnumerable<TripSearchDto>> GetByCountry(string country)
    {
        return await _dbContext.Trips
            .Include(x => x.Country)
            .Where(x => x.Country.Name.Equals(country, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => new TripSearchDto(x.Name, x.Country.Name, x.StartDate))
            .ToListAsync();
    }
}