using System.Globalization;
using BackendTripRecruitmentTask.Domain.Entities;
using BackendTripRecruitmentTask.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTripRecruitmentTask.Infrastructure.Seeders;

public static class DbSeeder
{
    public static async Task EnsureDatabaseSeeded(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var dbContext = scope.ServiceProvider.GetService<TripDbContext>() ??
                           throw new ArgumentNullException(nameof(TripDbContext));
        
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        SeedCountries(dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static void SeedCountries(TripDbContext dbContext)
    {
        var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        var countries = new Dictionary<string, string>();

        foreach (var culture in cultures)
        {
            var region = new RegionInfo(culture.LCID);
            // not sure if it contains all countries, it would be a better idea to just use external nuget/api
            // taking care of that
            countries.TryAdd(region.ThreeLetterISORegionName, region.EnglishName);
        }

        foreach (var country in countries)
        {
            dbContext.Countries.Add(Country.Create(country.Key, country.Value));
        }
    }
}