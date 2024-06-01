using BackendTripRecruitmentTask.Domain;
using BackendTripRecruitmentTask.Domain.Entities;
using BackendTripRecruitmentTask.Domain.Exceptions;

namespace BackendTripRecruitmentTask.UnitTests.DomainTests.EntitiesTests;

public class CountryTests
{
    [Fact]
    public void GetAllCountries_ReturnsNonEmptyList()
    {
        var countries = Country.GetAllCountries();

        Assert.NotEmpty(countries);
    }

    [Fact]
    public void GetAllCountries_ReturnsUniqueCountries()
    {
        var countries = Country.GetAllCountries().ToArray();

        Assert.Equal(countries.Count(), countries.Distinct().Count());
    }

    [Fact]
    public void GetAllCountries_CountryNameIsLongerThanXCharactersAndTrimIsOn_TrimNameToXCharacters()
    {
        var countries = Country.GetAllCountries();

        Assert.DoesNotContain(countries, x => x.Name.Length > Constants.MaximumCountryNameLength);
    }

    [Theory]
    [InlineData("CBA", "Random Country")]
    [InlineData("XXX", "وكذلك")]
    public void Create_ValidInput_ReturnsCountry(string code, string name)
    {
        var country = Country.Create(code, name);

        var expectedName = TrimToXCharacters(name, Constants.MaximumCountryNameLength);
        Assert.NotNull(country);
        Assert.Equal(code, country.ThreeLetterCode);
        Assert.Equal(expectedName, country.Name);
    }

    [Theory]
    [InlineData(null, "United States")]
    [InlineData("", "United States")]
    [InlineData("USA", null)]
    [InlineData("USA", "")]
    [InlineData("SA", "United States")]
    [InlineData("S", "United States")]
    [InlineData("USAA", "United States")]
    [InlineData("ABC", "Country Name But Longer Than Allowed")]
    public void Create_InvalidInput_ThrowsInputException(string threeLetterCode, string name)
    {
        Assert.Throws<InputException>(() => Country.Create(threeLetterCode, name));
    }

    private static string TrimToXCharacters(string input, int maxCharNumber)
    {
        var result = input;
        if (input.Length > maxCharNumber) result = result.Substring(0, maxCharNumber);

        return result;
    }
}