using BackendTripRecruitmentTask.Domain.Entities;
using BackendTripRecruitmentTask.Infrastructure.Data;
using BackendTripRecruitmentTask.Infrastructure.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BackendTripRecruitmentTask.UnitTests.InfrastructureTests.EntitiesConfigurationTests;

public class RegistrationEntityConfigurationTests
{
    [Fact]
    public void Configure_SetsConfigurationProperly()
    {
        var options = new DbContextOptionsBuilder<TripDbContext>()
            .UseInMemoryDatabase("test_database")
            .Options;

        using var dbContext = new TripDbContext(options);
        var builder = new ModelBuilder(new ConventionSet());
        var configuration = new RegistrationEntityConfiguration();

        configuration.Configure(builder.Entity<Registration>());

        var entityType = dbContext.Model.FindEntityType(typeof(Registration));
        Assert.NotNull(entityType);

        var idProperty = entityType.FindProperty(nameof(Registration.ID));
        Assert.NotNull(idProperty);
        Assert.True(idProperty.IsPrimaryKey());

        var emailProperty = entityType.FindProperty(nameof(Registration.Email));
        Assert.NotNull(emailProperty);
        Assert.False(emailProperty.IsNullable);

        var registeredAtProperty = entityType.FindProperty(nameof(Registration.RegisteredAt));
        Assert.NotNull(registeredAtProperty);
        Assert.False(registeredAtProperty.IsNullable);

        var tripNavigation = entityType.FindNavigation(nameof(Registration.Trip));
        Assert.NotNull(tripNavigation);
        Assert.False(tripNavigation.IsCollection);
        Assert.Equal(nameof(Registration.TripID), tripNavigation.ForeignKey.Properties.Single().Name);
        Assert.True(tripNavigation.ForeignKey.IsRequired);
    }
}