using BackendTripRecruitmentTask.Domain;
using BackendTripRecruitmentTask.Domain.Entities;
using BackendTripRecruitmentTask.Infrastructure.Data;
using BackendTripRecruitmentTask.Infrastructure.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BackendTripRecruitmentTask.UnitTests.InfrastructureTests.EntitiesConfigurationTests;

public class CountryEntityConfigurationTests
{
    [Fact]
    public void Configure_SetsConfigurationProperly()
    {
        var options = new DbContextOptionsBuilder<TripDbContext>()
            .UseInMemoryDatabase("test_database")
            .Options;

        using var dbContext = new TripDbContext(options);
        var builder = new ModelBuilder(new ConventionSet());
        var configuration = new CountryEntityConfiguration();

        configuration.Configure(builder.Entity<Country>());

        var entityType = dbContext.Model.FindEntityType(typeof(Country));
        Assert.NotNull(entityType);

        var codeProperty = entityType.FindProperty(nameof(Country.ThreeLetterCode));
        Assert.NotNull(codeProperty);
        Assert.Equal(3, codeProperty.GetMaxLength());
        Assert.True(codeProperty.IsPrimaryKey());

        var nameProperty = entityType.FindProperty(nameof(Country.Name));
        Assert.NotNull(nameProperty);
        Assert.Equal(Constants.MaximumCountryNameLength, nameProperty.GetMaxLength());
        Assert.False(nameProperty.IsNullable);
    }
}