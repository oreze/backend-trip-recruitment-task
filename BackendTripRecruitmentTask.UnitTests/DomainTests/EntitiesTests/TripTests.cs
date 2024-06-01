using BackendTripRecruitmentTask.Domain.Entities;
using BackendTripRecruitmentTask.Domain.Exceptions;

namespace BackendTripRecruitmentTask.UnitTests.DomainTests.EntitiesTests;

public class TripTests
{
    [Fact]
    public void Create_ValidInput_ReturnsTrip()
    {
        var name = "Test Trip";
        var description = "This is a test trip.";
        var startDate = DateTime.UtcNow.AddDays(1);
        var numberOfSeats = 50;
        var country = (Country?)Activator.CreateInstance(typeof(Country), true);

        var trip = Trip.Create(name, description, startDate, numberOfSeats, country!);

        Assert.NotNull(trip);
        Assert.Equal(name, trip.Name);
        Assert.Equal(description, trip.Description);
        Assert.Equal(startDate, trip.StartDate);
        Assert.Equal(numberOfSeats, trip.NumberOfSeats);
        Assert.Equal(country, trip.Country);
    }

    [Fact]
    public void Create_ValidInputWithoutDescription_ReturnsTrip()
    {
        var name = "Test Trip";
        var startDate = DateTime.UtcNow.AddDays(1);
        var numberOfSeats = 50;
        var country = (Country?)Activator.CreateInstance(typeof(Country), true);

        var trip = Trip.Create(name, null, startDate, numberOfSeats, country!);

        Assert.NotNull(trip);
        Assert.Equal(name, trip.Name);
        Assert.Equal(startDate, trip.StartDate);
        Assert.Equal(numberOfSeats, trip.NumberOfSeats);
        Assert.Equal(country, trip.Country);
        Assert.Null(trip.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("This is a test trip\nwith multiple lines.")]
    [InlineData("This is a test trip with more than fifty characters in the name.")]
    public void Create_InvalidName_ThrowsInputException(string name)
    {
        var description = "This is a test trip.";
        var startDate = DateTime.UtcNow.AddDays(1);
        var numberOfSeats = 50;
        var country = (Country?)Activator.CreateInstance(typeof(Country), true);

        Assert.Throws<InputException>(() => Trip.Create(name, description, startDate, numberOfSeats, country!));
    }

    [Fact]
    public void Create_EmptyDescription_ThrowsInputException()
    {
        var name = "Test Trip";
        var description = "   ";
        var startDate = DateTime.UtcNow.AddDays(1);
        var numberOfSeats = 50;
        var country = (Country?)Activator.CreateInstance(typeof(Country), true);

        Assert.Throws<InputException>(() => Trip.Create(name, description, startDate, numberOfSeats, country!));
    }

    [Fact]
    public void Create_PastStartDate_ThrowsInputException()
    {
        var name = "Test Trip";
        var description = "This is a test trip.";
        var startDate = DateTime.UtcNow.AddDays(-1);
        var numberOfSeats = 50;
        var country = (Country?)Activator.CreateInstance(typeof(Country), true);

        Assert.Throws<InputException>(() => Trip.Create(name, description, startDate, numberOfSeats, country!));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(99999)]
    public void Create_InvalidNumberOfSeats_ThrowsInputException(int numberOfSeats)
    {
        var name = "Test Trip";
        var description = "This is a test trip.";
        var startDate = DateTime.UtcNow.AddDays(1);
        var country = (Country?)Activator.CreateInstance(typeof(Country), true);

        Assert.Throws<InputException>(() => Trip.Create(name, description, startDate, numberOfSeats, country!));
    }

    [Fact]
    public void Create_NullCountry_ThrowsInputException()
    {
        var name = "Test Trip";
        var description = "This is a test trip.";
        var startDate = DateTime.UtcNow.AddDays(1);
        var numberOfSeats = 50;

        Assert.Throws<InputException>(() => Trip.Create(name, description, startDate, numberOfSeats, null!));
    }


    [Fact]
    public void Update_ValidInput_UpdatesTrip()
    {
        var country = (Country?)Activator.CreateInstance(typeof(Country), true);
        var trip = Trip.Create("Test Trip", "This is a test trip.", DateTime.UtcNow.AddDays(1), 50, country!);

        var newName = "Updated Trip";
        var newDescription = "This is an updated test trip.";
        var newStartDate = DateTime.UtcNow.AddDays(2);
        var newNumberOfSeats = 60;
        var newCountry = (Country?)Activator.CreateInstance(typeof(Country), true);

        trip.Update(newName, newDescription, newStartDate, newNumberOfSeats, newCountry);

        Assert.Equal(newName, trip.Name);
        Assert.Equal(newDescription, trip.Description);
        Assert.Equal(newStartDate, trip.StartDate);
        Assert.Equal(newNumberOfSeats, trip.NumberOfSeats);
        Assert.Same(newCountry, trip.Country);
    }

    [Fact]
    public void Update_NullInput_DoesNotUpdateTrip()
    {
        var country = (Country?)Activator.CreateInstance(typeof(Country), true);
        var trip = Trip.Create("Test Trip", "This is a test trip.", DateTime.UtcNow.AddDays(1), 50, country!);
        var originalName = trip.Name;
        var originalDescription = trip.Description;
        var originalStartDate = trip.StartDate;
        var originalNumberOfSeats = trip.NumberOfSeats;
        var originalCountry = trip.Country;

        trip.Update(null, null, null, null, null);

        Assert.Equal(originalName, trip.Name);
        Assert.Equal(originalDescription, trip.Description);
        Assert.Equal(originalStartDate, trip.StartDate);
        Assert.Equal(originalNumberOfSeats, trip.NumberOfSeats);
        Assert.Same(originalCountry, trip.Country);
    }
}