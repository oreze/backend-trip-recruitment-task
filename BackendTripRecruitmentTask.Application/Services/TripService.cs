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

    public async Task EditTrip(int id, EditTripDto editTripDto)
    {
        var tripToBeEdited = await _dbContext.Trips
            .Include(x => x.Country)
            .Include(x => x.Registrations)
            .FirstOrDefaultAsync(x => x.ID == id);
        
        if (tripToBeEdited == default)
            throw new NotFoundException("Could not find trip with ID {id}.");

        var newCountry = tripToBeEdited.Country;
        if (editTripDto.CountryName != default)
        {
            newCountry = await _dbContext.Countries.FirstOrDefaultAsync(x =>
                x.Name.Equals(editTripDto.CountryName, StringComparison.InvariantCultureIgnoreCase));

            if (newCountry == default)
                throw new InputException(nameof(editTripDto.CountryName),
                    $"The country name '{editTripDto.CountryName}' does not exist in the database. " +
                    $"Try using only English names, for reference check ISO 3166.");
        }

        if (tripToBeEdited.Registrations.Count > editTripDto.NumberOfSeats)
            throw new InputException(nameof(editTripDto.NumberOfSeats),
                $"Number of seats cannot be lower than number of already registered people. " +
                $"There are {tripToBeEdited.NumberOfSeats} registered users.");
        
        tripToBeEdited.Update(editTripDto.Name, editTripDto.Description, editTripDto.StartDate, 
            editTripDto.NumberOfSeats, newCountry);

        _dbContext.Trips.Update(tripToBeEdited);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteTrip(int id)
    {
        var tripToBeDeleted = await _dbContext.Trips.FirstOrDefaultAsync(x => x.ID == id);
        if (tripToBeDeleted == default)
            return false;
        
        var registrationsForTrip = _dbContext.Registrations
            .Where(x => x.TripID == id);

        _dbContext.RemoveRange(registrationsForTrip);
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

    public async Task<TripDetailsDto> GetDetails(int id)
    {
        var trip = await _dbContext.Trips
            .Include(x => x.Country)
            .Include(x => x.Registrations)
            .SingleOrDefaultAsync(x => x.ID == id);

        if (trip == default)
            throw new NotFoundException("Could not find trip with ID {id}.");

        var registrationsForTrip = trip.Registrations
            .Select(x => new RegistrationDetailsDto(x.Email, x.RegisteredAt));

        return new TripDetailsDto(trip.Name, trip.Country.Name, trip.Description,
            trip.StartDate, trip.NumberOfSeats, registrationsForTrip);
    }
    
    
    
}