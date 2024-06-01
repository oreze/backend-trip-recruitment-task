using BackendTripRecruitmentTask.Domain.Entities;
using BackendTripRecruitmentTask.Domain.Exceptions;

namespace BackendTripRecruitmentTask.UnitTests.DomainTests.EntitiesTests;

public class RegistrationTests
{
    [Fact]
    public void Create_ValidInput_ReturnsRegistration()
    {
        // Arrange
        var email = "test@example.com";
        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);

        // Act
        var registration = Registration.Create(email, trip!);

        // Assert
        Assert.NotNull(registration);
        Assert.Equal(email, registration.Email);
        Assert.Equal(trip, registration.Trip);
        Assert.True(DateTime.UtcNow - registration.RegisteredAt < TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@em.com")]
    [InlineData("@")]
    public void Create_InvalidEmail_ThrowsInputException(string email)
    {
        // Arrange
        var trip = (Trip?)Activator.CreateInstance(typeof(Trip), true);

        // Act and Assert
        Assert.Throws<InputException>(() => Registration.Create(email, trip!));
    }

    [Fact]
    public void Create_NullTrip_ThrowsInputException()
    {
        // Arrange
        var email = "test@example.com";

        // Act and Assert
        Assert.Throws<InputException>(() => Registration.Create(email, null!));
    }
}