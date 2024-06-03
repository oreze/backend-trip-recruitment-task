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

        await SeedCountries(dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedCountries(TripDbContext dbContext)
    {
        var countries = Country.GetAllCountries();
        await dbContext.Countries.AddRangeAsync(countries);
    }
}