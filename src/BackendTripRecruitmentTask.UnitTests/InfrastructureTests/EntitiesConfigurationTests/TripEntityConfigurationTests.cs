using BackendTripRecruitmentTask.Domain;
using BackendTripRecruitmentTask.Domain.Entities;
using BackendTripRecruitmentTask.Infrastructure.Data;
using BackendTripRecruitmentTask.Infrastructure.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BackendTripRecruitmentTask.UnitTests.InfrastructureTests.EntitiesConfigurationTests;

public class TripEntityConfigurationTests
{
    [Fact]
    public void Configure_SetsConfigurationProperly()
    {
        var options = new DbContextOptionsBuilder<TripDbContext>()
            .UseInMemoryDatabase("test_database")
            .Options;

        using var dbContext = new TripDbContext(options);
        var builder = new ModelBuilder(new ConventionSet());
        var configuration = new TripEntityConfiguration();

        configuration.Configure(builder.Entity<Trip>());

        var entityType = dbContext.Model.FindEntityType(typeof(Trip));
        Assert.NotNull(entityType);

        var idProperty = entityType.FindProperty(nameof(Trip.ID));
        Assert.NotNull(idProperty);
        Assert.True(idProperty.IsPrimaryKey());

        var nameProperty = entityType.FindProperty(nameof(Trip.Name));
        Assert.NotNull(nameProperty);
        Assert.Equal(Constants.MaximumTripNameLength, nameProperty.GetMaxLength());
        Assert.False(nameProperty.IsNullable);
        Assert.True(nameProperty.IsUniqueIndex());

        var descriptionProperty = entityType.FindProperty(nameof(Trip.Description));
        Assert.NotNull(descriptionProperty);

        var startDateProperty = entityType.FindProperty(nameof(Trip.StartDate));
        Assert.NotNull(startDateProperty);
        Assert.False(startDateProperty.IsNullable);

        var numberOfSeatsProperty = entityType.FindProperty(nameof(Trip.NumberOfSeats));
        Assert.NotNull(numberOfSeatsProperty);
        Assert.False(numberOfSeatsProperty.IsNullable);

        var registrationsNavigation = entityType.FindNavigation(nameof(Trip.Registrations));
        Assert.NotNull(registrationsNavigation);
        Assert.True(registrationsNavigation.IsCollection);
        Assert.Equal(nameof(Registration.TripID), registrationsNavigation.ForeignKey.Properties.Single().Name);
        Assert.True(registrationsNavigation.ForeignKey.IsRequired);

        var countryNavigation = entityType.FindNavigation(nameof(Trip.Country));
        Assert.NotNull(countryNavigation);
        Assert.False(countryNavigation.IsCollection);
        Assert.Equal(nameof(Trip.CountryThreeLetterCode), countryNavigation.ForeignKey.Properties.Single().Name);
        Assert.True(countryNavigation.ForeignKey.IsRequired);
    }
}